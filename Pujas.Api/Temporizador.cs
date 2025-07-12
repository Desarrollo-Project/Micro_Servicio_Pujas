using System.Text;
using Newtonsoft.Json;
using Pujas.Dominio.Repositorios;

namespace Pujas.Api
{
    public class Temporizador : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly HttpClient _httpClient;
        private readonly HttpClient _pujasHttpClient;
        private Timer _timer;

        public Temporizador(IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory)
        {
            _scopeFactory = scopeFactory;
            _httpClient = httpClientFactory.CreateClient("SubastasClient");
            _pujasHttpClient = httpClientFactory.CreateClient("PujasClient");

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Servicio Temporizador iniciado.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Obtener_Id_Subastas_Activas");
                response.EnsureSuccessStatusCode();
                var scope = _scopeFactory.CreateScope();
                string responseBody = await response.Content.ReadAsStringAsync();

                var _repositorio_Pujas  = scope.ServiceProvider.GetRequiredService<IRepositorio_Pujas_Lectura>();

                List<string> subastasActivas = JsonConvert.DeserializeObject<List<string>>(responseBody);

                foreach (var subastaId in subastasActivas)
                {
                    Console.WriteLine($"- ID Subasta: {subastaId}");
                    DateTime? fechaUltimaPuja = await _repositorio_Pujas.Obtener_Fecha_Ultima_Puja(subastaId);

                    if (fechaUltimaPuja.HasValue)
                    {
                        TimeSpan tiempoTranscurrido = DateTime.UtcNow - fechaUltimaPuja.Value;

                        Console.WriteLine($"  Última puja para {subastaId} fue hace: {tiempoTranscurrido.TotalSeconds:F2} segundos.");

                        if (tiempoTranscurrido.TotalSeconds > 15)
                        {
                            var jsonContent = JsonConvert.SerializeObject(subastaId);
                            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                            var postResponse = await _pujasHttpClient.PostAsync("Hacer_Pujas_Automaticas", content);
                            postResponse.EnsureSuccessStatusCode();
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de solicitud HTTP al obtener subastas activas: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado en la tarea periódica: {ex.Message}");
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Servicio Temporizador detenido.");
            // Detiene el temporizador, impidiendo que se dispare de nuevo.
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    }
}

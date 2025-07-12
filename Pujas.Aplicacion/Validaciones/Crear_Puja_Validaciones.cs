using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pujas.Aplicacion.DTO;
using Pujas.Dominio.Excepciones_Personalizadas;

namespace Pujas.Aplicacion.Validaciones
{
    public class Crear_Puja_Validaciones
    {
        private readonly HttpClient _httpClient;
        private const string Base_Api_Subastas = "http://localhost:5247/api/Subastas/";

        public Crear_Puja_Validaciones(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(Base_Api_Subastas);
        }
        
        public void Validar_Datos(Crear_Puja_Automatica_DTO dto)
        {
            if (dto.Maximo_Monto < dto.Incrementos)
            {
                throw new Monto_Maximo_Menor_Monto_Base_Subasta(
                    "El Monto maximo no puede ser menor al precio base de la subasta");
            }

            if (dto.Incrementos > dto.Maximo_Monto)
            {
                throw new Incremento_Mayor_Monto_Maximo(
                    "El incremento no puede ser mayor al monto maximo de la puja automatica");
            }

            if (dto.Incrementos <= 0)
            { throw new Incremento_Menor_O_Igual_Cero("El incremento debe ser mayor a cero"); }
        }

        public void Validar_Datos_Puja(Crear_Puja_DTO dto)
        {
            if (dto.Monto <= 0)
            { throw new ArgumentException("El monto de la puja debe ser mayor a cero"); }
            if (string.IsNullOrEmpty(dto.Id_Subasta))
            { throw new ArgumentException("El ID de la subasta no puede estar vacío"); }
            if (string.IsNullOrEmpty(dto.Id_Postor))
            { throw new ArgumentException("El ID del postor no puede estar vacío"); }
        }

        public void Validar_Monto_Puja_Con_Ultima_Puja(decimal monto)
        {


        }

        public async Task<bool> Subasta_Esta_Activa_Async(string idSubasta)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Obtener_Estado_Subasta?idSubasta={idSubasta}");
                response.EnsureSuccessStatusCode(); 
                var estadoSubastaString = await response.Content.ReadAsStringAsync();

                return estadoSubastaString.Trim().Equals("Activa", StringComparison.OrdinalIgnoreCase);
            }
            catch (HttpRequestException ex) {
            
                return false;
            }
            catch (Exception ex) 
            {
                return false;
            }
        }

    }
}

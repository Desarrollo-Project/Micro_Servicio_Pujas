using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Pujas.Aplicacion.DTO;

namespace Pujas.Api.Controllers
{
    public class Pujas_Hub : Hub
    { 
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> UsuariosPorSubasta = new();
        private readonly ILogger<Pujas_Hub> _logger;

        public Pujas_Hub(ILogger<Pujas_Hub> log)
        {_logger = log;}

        public async Task EstablecerConexion(string idSubasta, string idUsuario)
        {
            Context.Items["idSubasta"] = idSubasta;
            Context.Items["idUsuario"] = idUsuario;
            await Groups.AddToGroupAsync(Context.ConnectionId, idSubasta);
            var usersInAuction = UsuariosPorSubasta.GetOrAdd(idSubasta, _ => new ConcurrentDictionary<string, string>());
            usersInAuction[Context.ConnectionId] = idUsuario;
            _logger.LogInformation("Usuario {UsuarioId} conectado a subasta {SubastaId}", idUsuario, idSubasta);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.Items.TryGetValue("idSubasta", out var idSubastaObj) && idSubastaObj is string idSubasta &&
                Context.Items.TryGetValue("idUsuario", out var idUsuarioObj) && idUsuarioObj is string idUsuario)
            {
                if (UsuariosPorSubasta.TryGetValue(idSubasta, out var usersInAuction))
                {
                    if (usersInAuction.TryRemove(Context.ConnectionId, out _))
                    {
                        _logger.LogInformation("Usuario {UsuarioId} desconectado de {SubastaId}", idUsuario, idSubasta);

                        if (usersInAuction.IsEmpty)
                        {
                            UsuariosPorSubasta.TryRemove(idSubasta, out _);
                            _logger.LogInformation("Subasta {SubastaId} sin usuarios activos", idSubasta);
                        }
                    }
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

    }

}

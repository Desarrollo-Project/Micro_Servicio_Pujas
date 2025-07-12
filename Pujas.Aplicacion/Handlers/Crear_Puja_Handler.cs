using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Pujas.Aplicacion.Commands;
using Pujas.Dominio.Entidades;
using Pujas.Dominio.Eventos_de_Dominio;
using Pujas.Dominio.Objetos_De_Valor;
using Pujas.Dominio.Repositorios;

namespace Pujas.Aplicacion.Handlers
{
    public class Crear_Puja_Handler : IRequestHandler<Crear_Puja_Command, Guid>
    {
        private readonly IRepositorio_Pujas repo_pujas;
        private readonly MassTransit.IPublishEndpoint _publish;

        public Crear_Puja_Handler(IPublishEndpoint p, IRepositorio_Pujas repo) { _publish=p; repo_pujas = repo;}

        public async Task<Guid> Handle(Crear_Puja_Command request, CancellationToken cancellationToken)
        {
            var dto = request.dto;
            var id = Guid.NewGuid();
            var puja = new Puja(
                id,
                new Id_Postor_VO(dto.Id_Postor),
                new Id_Subasta_VO(dto.Id_Subasta),
                new Fecha_Puja_VO(dto.Fecha_Puja),
                new Monto_Total_VO(dto.Monto),
                new Incremento_VO(dto.Incremento));

            try
            {  
                await repo_pujas.Crear_Subasta(puja);
               var evento = new Crear_Puja_Evento(
                    puja.Id.ToString(),
                    puja.Id_Subasta.id_Subasta,
                    puja.Id_Postor.id_postor,
                    puja.Fecha_Puja.fecha,
                    puja.Monto.Monto_Total,
                    puja.Incremento.Incremento
                );
                await _publish.Publish(evento);

            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear la puja: {ex.Message}");
            }
            return puja.Id;
        }
    }
}

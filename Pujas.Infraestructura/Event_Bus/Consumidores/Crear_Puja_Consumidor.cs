using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Pujas.Dominio.Entidades;
using Pujas.Dominio.Eventos_de_Dominio;
using Pujas.Dominio.Objetos_De_Valor;
using Pujas.Dominio.Repositorios;

namespace Pujas.Infraestructura.Event_Bus.Consumidores
{
    public class Crear_Puja_Consumidor: IConsumer<Crear_Puja_Evento>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRepositorio_Pujas_Lectura _repositorioPujas;

        public Crear_Puja_Consumidor(IPublishEndpoint publishEndpoint, IRepositorio_Pujas_Lectura repo)
        { _publishEndpoint = publishEndpoint; _repositorioPujas = repo; }

        public async Task Consume(ConsumeContext<Crear_Puja_Evento> context)
        {
            var datos = context.Message;

            var puja = new Puja_Mongo(
                datos.Id.ToString(),
                new Id_Postor_VO(datos.Id_Postor),
                new Id_Subasta_VO(datos.Id_Subasta),
                new Fecha_Puja_VO(datos.Fecha_Puja),
                new Monto_Total_VO(datos.Monto),
                new Incremento_VO(datos.Incremento)
            );

            // Guardar la puja en el repositorio
            await _repositorioPujas.Crear_Puja(puja);
        }
    }
}

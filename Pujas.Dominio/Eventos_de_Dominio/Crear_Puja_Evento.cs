using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Pujas.Dominio.Eventos_de_Dominio
{
    public  class Crear_Puja_Evento: INotification
    {
        public string Id { get; set; }
        public string Id_Subasta { get; set; }
        public string Id_Postor { get; set; }
        public DateTime Fecha_Puja { get; set; }
        public decimal Monto { get; set; }
        public decimal Incremento { get; set; }
        public Crear_Puja_Evento(string Id, string Id_Subasta, string Id_Postor, DateTime Fecha_Puja, decimal Monto, decimal Incremento)
        {
            this.Id = Id; this.Id_Subasta = Id_Subasta;
            this.Id_Postor = Id_Postor; this.Fecha_Puja = Fecha_Puja;

            this.Monto = Monto;
            this.Incremento = Incremento;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Pujas.Aplicacion.DTO
{
    public class Crear_Puja_DTO: IRequest<Guid>
    {
        public Guid Id { get; set; }
        public string Id_Subasta { get; set; }
        public string Id_Postor { get; set; }
        public DateTime Fecha_Puja { get; set; }
        public decimal Monto { get; set; }
        public decimal Incremento { get; set; }

        //constructor con todos los atributos 

        public Crear_Puja_DTO(Guid id, string id_Subasta, string id_Postor, DateTime fecha_Puja, decimal monto, decimal incremento)
        {
            Id = id; Id_Subasta = id_Subasta;
            Id_Postor = id_Postor; Fecha_Puja = fecha_Puja;
            Monto = monto; Incremento = incremento;
        }
    }
}

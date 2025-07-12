using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pujas.Aplicacion.DTO
{
    public class Crear_Puja_Automatica_DTO
    {
        public Guid Id { get; set; }
        public string Id_Subasta { get; set; }
        public string Id_Postor { get; set; }
        public decimal Maximo_Monto { get; set; }
        public decimal Incrementos { get; set; }
        public DateTime Fecha_Creacion_Puja_Automatica { get; set; }

    }
}

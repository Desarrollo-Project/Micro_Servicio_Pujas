using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Pujas.Dominio.Objetos_De_Valor;

namespace Pujas.Dominio.Entidades
{
    public class Puja
    {
        public Guid Id{get; private set; }
        public Id_Postor_VO Id_Postor { get; private set; }
        public Id_Subasta_VO Id_Subasta { get; private set; }
        public Fecha_Puja_VO Fecha_Puja { get; private set; }

        // 
        public Monto_Total_VO Monto { get; private set; }
        public  Incremento_VO Incremento { get; private set; }

        // Constructor 
        public Puja(Guid id,Id_Postor_VO id_postor, Id_Subasta_VO id_subasta, Fecha_Puja_VO fecha_puja, Monto_Total_VO monto,Incremento_VO incremento)
        {
            // Strings(Guids) y DateTime 
            Id = id;
            Id_Postor = id_postor;
            Id_Subasta = id_subasta;
            Fecha_Puja = fecha_puja;

            /// Todos los decimales 
            Monto = monto;
            Incremento = incremento;
        }

        // Lo necesita EF 
        private Puja() { }

    }
}

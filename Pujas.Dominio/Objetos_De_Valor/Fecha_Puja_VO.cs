using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pujas.Dominio.Excepciones_Personalizadas;

namespace Pujas.Dominio.Objetos_De_Valor
{
    public class Fecha_Puja_VO
    { 
        public DateTime fecha { get; private set; }

        public Fecha_Puja_VO(DateTime f)
        {
            if (f.CompareTo(DateTime.UtcNow) > 0)throw new Fecha_Invalida_Puja("La fecha de la puja no coincide con la fecha actual");
            fecha = f;
        }
        public override string ToString()
        {return fecha.ToString("dd/MM/yyyy"); }

    }
}

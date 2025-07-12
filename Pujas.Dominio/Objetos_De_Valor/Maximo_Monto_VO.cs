using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pujas.Dominio.Excepciones_Personalizadas;

namespace Pujas.Dominio.Objetos_De_Valor
{
    public class Maximo_Monto_VO
    {
        public decimal monto_max { get; private set; }
        public Maximo_Monto_VO(decimal mm)
        {
            if (mm <= 0) throw new Monto_Total_Invalido("El Monto maximo debe ser un valor superior a 0 ");
            monto_max = mm;
        }
        public decimal ToDecimal() { return monto_max; }
    }
}

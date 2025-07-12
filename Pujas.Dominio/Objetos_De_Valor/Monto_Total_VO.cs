using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pujas.Dominio.Excepciones_Personalizadas;

namespace Pujas.Dominio.Objetos_De_Valor
{
    public class Monto_Total_VO
    {
        public decimal Monto_Total { get; private set; }

        public Monto_Total_VO(decimal monto)
        {
            if (monto < 0) throw new Monto_Total_Invalido("El monto no puede ser negativo");
            Monto_Total = monto;
        }
        public decimal ToDecimal()
        { return Monto_Total; }
    }
}

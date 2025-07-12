using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pujas.Dominio.Excepciones_Personalizadas;

namespace Pujas.Dominio.Objetos_De_Valor
{
    public class Incremento_VO
    {
        public decimal Incremento { get; private set; }
        public Incremento_VO(decimal inc)
        {
            if (inc <= 0) throw new Incremento_Invalido("El Incremento minimo debe ser un valor superior a 0 ");
            Incremento = inc;
        }
        public decimal ToDecimal() { return Incremento; }
    }
}

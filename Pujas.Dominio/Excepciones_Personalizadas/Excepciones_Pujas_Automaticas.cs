using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pujas.Dominio.Excepciones_Personalizadas
{
    public class Monto_Maximo_Menor_Monto_Base_Subasta:Exception
    {
        public Monto_Maximo_Menor_Monto_Base_Subasta(string mensaje) : base(mensaje) { }
    }

    public class Incremento_Mayor_Monto_Maximo : Exception
    {
        public Incremento_Mayor_Monto_Maximo(string mensaje) : base(mensaje) { }
    }

    public class Incremento_Menor_O_Igual_Cero : Exception
    {
        public Incremento_Menor_O_Igual_Cero(string mensaje) : base(mensaje) { }
    }

}

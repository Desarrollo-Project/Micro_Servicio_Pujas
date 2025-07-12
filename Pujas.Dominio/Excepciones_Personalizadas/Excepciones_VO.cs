using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pujas.Dominio.Excepciones_Personalizadas
{
    public class Fecha_Invalida_Puja: Exception { 
        public Fecha_Invalida_Puja(string mensaje) : base(mensaje) { }
    }

    public class Id_Postor_Invalido : Exception
    {
        public Id_Postor_Invalido(string mensaje) : base(mensaje) { }
    }

    public class Id_Subasta_Invalido : Exception
    {
        public Id_Subasta_Invalido(string mensaje) : base(mensaje) { }
    }

    public class Incremento_Invalido : Exception
    {
        public Incremento_Invalido(string mensaje) : base(mensaje) { }
    }

    public class Monto_Total_Invalido : Exception
    {
        public Monto_Total_Invalido(string mensaje) : base(mensaje) { }
    }
}

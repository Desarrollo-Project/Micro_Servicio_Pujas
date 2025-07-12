using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pujas.Dominio.Excepciones_Personalizadas;

namespace Pujas.Dominio.Objetos_De_Valor
{
    public class Id_Subasta_VO
    {
        public string id_Subasta { get; private set; }
        public Id_Subasta_VO(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new Id_Subasta_Invalido("el id de la subasta no puede ser vacio o nulo");
            id_Subasta = id;
        }
        public override string ToString()
        {return id_Subasta.ToString(); }
    }
}

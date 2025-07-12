using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pujas.Dominio.Excepciones_Personalizadas;

namespace Pujas.Dominio.Objetos_De_Valor
{
    public class Id_Postor_VO
    {
        public string id_postor { get; private set; }
        public Id_Postor_VO(string id) 
        { 
            if(string.IsNullOrEmpty(id)) throw new Id_Postor_Invalido ("El id del postor no puede ser vacio o nulo");
            id_postor = id;
        }
        public override string ToString(){return id_postor.ToString();}

    }
}

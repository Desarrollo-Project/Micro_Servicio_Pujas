using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pujas.Dominio.Entidades;

namespace Pujas.Dominio.Repositorios
{
    public interface IRepositorio_Pujas
    {
        Task Crear_Subasta(Puja puja);
    }
}

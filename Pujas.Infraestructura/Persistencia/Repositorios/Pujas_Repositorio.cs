using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pujas.Dominio.Entidades;
using Pujas.Dominio.Repositorios;

namespace Pujas.Infraestructura.Persistencia.Repositorios
{
    public class Pujas_Repositorio: IRepositorio_Pujas
    {

        private readonly App_Db_Context _context;

        public Pujas_Repositorio(App_Db_Context context) { _context = context; }
        
        /// <summary>
        /// Agrega una nueva puja a la base de datos.
        /// </summary>
        /// <param name="puja">El objeto Puja a ser agregado.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el objeto puja es nulo.</exception>
        public async Task Crear_Subasta(Puja puja)
        {

            if (puja == null) throw new ArgumentNullException();
            _context.pujas.Add(puja);
            await _context.SaveChangesAsync();

        }
    }
}

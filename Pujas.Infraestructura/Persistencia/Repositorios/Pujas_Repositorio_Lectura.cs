using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Pujas.Dominio.Entidades;
using Pujas.Dominio.Repositorios;

namespace Pujas.Infraestructura.Persistencia.Repositorios
{
    public class Pujas_Repositorio_Lectura: IRepositorio_Pujas_Lectura
    {
        private readonly IMongoCollection<Dominio.Entidades.Puja_Mongo> Pujas_Collection;
        private readonly IMongoCollection<Dominio.Entidades.Puja_Mongo> Pujas_Automaticas_Collection;

        public Pujas_Repositorio_Lectura(IMongoDatabase database)
        {Pujas_Collection = database.GetCollection<Dominio.Entidades.Puja_Mongo>("pujas"); 
        Pujas_Automaticas_Collection= database.GetCollection<Dominio.Entidades.Puja_Mongo>("Pujas_Automaticas"); }

        /// <summary>
        /// Inserta una nueva puja manual en la colección de pujas.
        /// </summary>
        /// <param name="puja">El objeto de la puja a ser creado en la base de datos.</param>
        /// <returns>Una tarea que representa la operación asíncrona de inserción.</returns>
        public async Task Crear_Puja(Dominio.Entidades.Puja_Mongo puja)
        {
            await Pujas_Collection.InsertOneAsync(puja);
            Console.WriteLine("Insercion exitosa;;");
        }

        //Obtener_Pujas_Subasta_Por_Orden de fecha ascendente


        /// <summary>
        /// Obtiene una lista de todas las pujas manuales para una subasta específica, ordenadas por fecha de forma ascendente.
        /// </summary>
        /// <param name="id_Subasta">El identificador único de la subasta.</param>
        /// <returns>Una tarea que devuelve una lista de objetos Puja_Mongo.</returns>
        public async Task<List<Dominio.Entidades.Puja_Mongo>> Obtener_Pujas_Subasta_Por_Orden(string id_Subasta)
        {
            var filter_Builder = Builders<Dominio.Entidades.Puja_Mongo>.Filter;
            var filter = filter_Builder.Eq(p => p.Id_Subasta.id_Subasta, id_Subasta);
            // Ordenar de forma ascendente 
            var sort = Builders<Dominio.Entidades.Puja_Mongo>.Sort.Ascending(p => p.Fecha_Puja.fecha);
            // Ejecuta la consulta y devuelve la lista
            return await Pujas_Collection.Find(filter).Sort(sort).ToListAsync();
        }

        /// <summary>
        /// Inserta una nueva configuración de puja automática en la colección correspondiente.
        /// </summary>
        /// <param name="puja">El objeto de configuración de la puja automática a ser guardado.</param>
        /// <returns>Una tarea que representa la operación asíncrona de inserción.</returns>

        public async Task Crear_Puja_Automatizada(Dominio.Entidades.Puja_Mongo puja)
        {
            await Pujas_Automaticas_Collection.InsertOneAsync(puja);
            Console.WriteLine("Insercion exitosa de puja automatizada;;");
        }

        /// <summary>
        /// Obtiene el monto de la última puja (la más alta) realizada en una subasta.
        /// </summary>
        /// <param name="id_Subasta">El identificador único de la subasta.</param>
        /// <returns>
        /// Una tarea que devuelve el valor decimal del monto de la última puja.
        /// Devuelve 0 si no existen pujas para esa subasta.
        /// </returns>
        public async Task<decimal> Obtener_Ultimo_Monto_Puja(string id_Subasta)
        {
            var filter = Builders<Dominio.Entidades.Puja_Mongo>.Filter.Eq(p => p.Id_Subasta.id_Subasta, id_Subasta);
            var sort = Builders<Dominio.Entidades.Puja_Mongo>.Sort.Descending(p => p.Fecha_Puja);
            var ultimaPuja = await Pujas_Collection.Find(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();
            return ultimaPuja?.Monto.Monto_Total ?? 0;
        }

        /// <summary>
        /// Obtiene una lista de todas las configuraciones de pujas automáticas para una subasta, ordenadas por fecha de creación ascendente.
        /// </summary>
        /// <param name="id_Subasta">El identificador único de la subasta.</param>
        /// <returns>Una tarea que devuelve una lista de objetos Puja_Mongo que representan las configuraciones automáticas.</returns>
        public async Task<List<Dominio.Entidades.Puja_Mongo>> Obtener_Pujas_Automaticas_Subasta_Por_Orden(string id_Subasta)
        {
            var filter_Builder = Builders<Dominio.Entidades.Puja_Mongo>.Filter;
            var filter = filter_Builder.Eq(p => p.Id_Subasta.id_Subasta, id_Subasta);
            // Ordenar de forma ascendente 
            var sort = Builders<Dominio.Entidades.Puja_Mongo>.Sort.Ascending(p => p.Fecha_Puja.fecha);
            // Ejecuta la consulta y devuelve la lista
            return await Pujas_Automaticas_Collection.Find(filter).Sort(sort).ToListAsync();
        }


        /// <summary>
        /// Obtiene el identificador del postor que realizó la última puja en una subasta.
        /// </summary>
        /// <param name="id_Subasta">El identificador único de la subasta.</param>
        /// <returns>
        /// Una tarea que devuelve el ID del último postor como una cadena.
        /// Devuelve una cadena vacía si no hay pujas.
        /// </returns>
        public async Task<string> Obtener_Id_Postor_Ultima_Puja(string id_Subasta)
        {
            var filter = Builders<Dominio.Entidades.Puja_Mongo>.Filter.Eq(p => p.Id_Subasta.id_Subasta, id_Subasta);
            var sort = Builders<Dominio.Entidades.Puja_Mongo>.Sort.Descending(p => p.Fecha_Puja);
            var ultimaPuja = await Pujas_Collection.Find(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();
            return ultimaPuja.Id_Postor.id_postor ?? string.Empty;
        }

        /// <summary>
        /// Obtiene la fecha y hora de la última puja realizada en una subasta.
        /// </summary>
        /// <param name="idSubasta">El identificador único de la subasta.</param>
        /// <returns>
        /// Una tarea que devuelve un objeto DateTime? (nullable).
        /// Devuelve el valor de la fecha si existe una puja, o null si no hay pujas.
        /// </returns>
        public async Task<DateTime?> Obtener_Fecha_Ultima_Puja(string idSubasta)
        {
            var filter = Builders<Dominio.Entidades.Puja_Mongo>.Filter.Eq(p => p.Id_Subasta.id_Subasta, idSubasta);
            var sort = Builders<Dominio.Entidades.Puja_Mongo>.Sort.Descending(p => p.Fecha_Puja.fecha);
            var ultimaPuja = await Pujas_Collection.Find(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();
            return ultimaPuja?.Fecha_Puja?.fecha;
        }

        /// <summary>
        /// Comprueba si un usuario específico tiene al menos una configuración de puja automática activa.
        /// </summary>
        /// <param name="id_Usuario">El identificador único del usuario (postor).</param>
        /// <returns>
        /// Una tarea que devuelve 'true' si el usuario tiene al menos una puja automática,
        /// y 'false' en caso contrario.
        /// </returns>
        public async Task<bool> Comprobar_Si_Usuario_Tiene_Puja_Automatica(string id_Usuario)
        {

            var filter = Builders<Dominio.Entidades.Puja_Mongo>.Filter.Eq(p => p.Id_Postor.id_postor, id_Usuario);
            var count = await Pujas_Automaticas_Collection.CountDocumentsAsync(filter);
            return count > 0;

        }

         /// <summary>
        /// Obtiene los datos clave de la última puja de una subasta: el monto y el nombre del ofertante.
        /// </summary>
        /// <param name="id_subasta">El identificador único de la subasta.</param>
        /// <returns>
        /// Una tarea que devuelve una tupla con el monto de la última puja (decimal) y el nombre del ofertante (string).
        /// Si no hay pujas, devuelve (0, string.Empty).
        /// </returns>

        public async Task<(decimal ultimaPuja, string nombreOfertante)> Obtener_Datos_Ultima_Puja(string id_subasta)
        {
            var filter = Builders<Dominio.Entidades.Puja_Mongo>.Filter.Eq(p => p.Id_Subasta.id_Subasta, id_subasta);
            var sort = Builders<Dominio.Entidades.Puja_Mongo>.Sort.Descending(p => p.Fecha_Puja);
            var ultimaPuja = await Pujas_Collection.Find(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();
            if (ultimaPuja != null)
            {
                return (ultimaPuja.Monto.Monto_Total, ultimaPuja.Id_Postor.id_postor);
            }
            return (0, string.Empty);
        }

    }

}

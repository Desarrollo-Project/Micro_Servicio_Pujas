using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pujas.Dominio.Entidades;

namespace Pujas.Dominio.Repositorios
{
    public interface IRepositorio_Pujas_Lectura
    {
        Task Crear_Puja(Puja_Mongo puja);
        Task Crear_Puja_Automatizada(Puja_Mongo puja);
        Task<List<Puja_Mongo>> Obtener_Pujas_Subasta_Por_Orden(string Id_subasta);
        Task<Decimal> Obtener_Ultimo_Monto_Puja(string Id_subasta);
        Task<string> Obtener_Id_Postor_Ultima_Puja(string Id_subasta);
        Task <List<Puja_Mongo>> Obtener_Pujas_Automaticas_Subasta_Por_Orden(string Id_subasta);
        Task<DateTime?> Obtener_Fecha_Ultima_Puja(string idSubasta);

        Task<bool> Comprobar_Si_Usuario_Tiene_Puja_Automatica(string id_Usuario);

        Task<(decimal ultimaPuja, string nombreOfertante)> Obtener_Datos_Ultima_Puja(string Id_subasta);
    }
}

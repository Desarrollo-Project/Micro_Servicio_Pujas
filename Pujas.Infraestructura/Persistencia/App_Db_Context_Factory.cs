using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Pujas.Infraestructura.Persistencia
{
    public class App_Db_Context_Factory : IDesignTimeDbContextFactory<App_Db_Context>
    {
        public App_Db_Context CreateDbContext(string[] args)
        {
            var configuracion = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
            var Conexion_Postgres = configuracion.GetConnectionString("Postgres");

            var options_Builder = new DbContextOptionsBuilder<App_Db_Context>();
            options_Builder.UseNpgsql(Conexion_Postgres);
            return new App_Db_Context(options_Builder.Options);
        }
    }

}

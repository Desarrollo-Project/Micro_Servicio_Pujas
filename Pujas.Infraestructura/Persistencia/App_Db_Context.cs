using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pujas.Dominio.Entidades;
using Pujas.Dominio.Objetos_De_Valor;

namespace Pujas.Infraestructura.Persistencia
{
    public class App_Db_Context: DbContext
    {
        public DbSet<Puja> pujas { get; set; }
        public App_Db_Context(DbContextOptions<App_Db_Context> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Puja>()
                .ToTable("Subasta");

            modelBuilder.Entity<Puja>()
                .HasKey(s => s.Id);

            /////////// Definicion de los atributos ///////////
            /// 
            modelBuilder.Entity<Puja>()
                .Property(p => p.Id_Subasta)
                .HasColumnName("Id_Subasta")
                .HasConversion(
                    v => v.id_Subasta,
                    v => new Id_Subasta_VO(v)
                );

            modelBuilder.Entity<Puja>()
                .Property(p => p.Id_Postor)
                .HasColumnName("Id_Postor")
                .HasConversion(
                    v => v.id_postor,
                    v => new Id_Postor_VO(v)
                );

            modelBuilder.Entity<Puja>()
                .Property(p => p.Fecha_Puja)
                .HasColumnName("Fecha")
                .HasConversion(
                    v => v.fecha,
                    v => new Fecha_Puja_VO(v)
                );

            modelBuilder.Entity<Puja>()
                .Property(p => p.Incremento)
                .HasColumnName("Incremento")
                .HasConversion(
                    v => v.Incremento,
                    v => new Incremento_VO(v)
                );

            modelBuilder.Entity<Puja>()
                .Property(p => p.Monto)
                .HasColumnName("Monto")
                .HasConversion(
                    v => v.Monto_Total,
                    v => new Monto_Total_VO(v)
                );



        }
    }
}

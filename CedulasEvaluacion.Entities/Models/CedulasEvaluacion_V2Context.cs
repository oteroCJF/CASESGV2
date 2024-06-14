using System;
using CedulasEvaluacion.Entities.MCatalogoServicios;
using CedulasEvaluacion.Entities.MFinancieros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class CedulasEvaluacion_V2Context : DbContext
    {
        public CedulasEvaluacion_V2Context()
        {
        }

        public CedulasEvaluacion_V2Context(DbContextOptions<CedulasEvaluacion_V2Context> options)
            : base(options)
        {
        }

        public virtual DbSet<CatalogoServicios> CatalogoServicios { get; set; }
        public virtual DbSet<Inmueble> Inmueble { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-KJ6A08Q;Initial Catalog=CedulasEvaluacion_V2;User Id=sa;Password=Yael.123;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

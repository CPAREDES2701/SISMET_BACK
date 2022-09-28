using ApiDavis.Core.DTOs;
using ApiDavis.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Infraestructure.Repositories
{
    public class ApplicationDbContext: DbContext
    {        
        public ApplicationDbContext(DbContextOptions options):base (options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Floral>().HasNoKey();
        }

        public DbSet<Floral> Floral { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Estacion> Estacion { get; set; }
        public DbSet<DataDavisEntiti> DataDavis { get; set; }
        public DbSet<EstacionMaestro> EstacionMaestro { get; set; }
    }
}

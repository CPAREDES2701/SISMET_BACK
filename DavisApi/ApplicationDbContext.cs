using DavisApi.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DavisApi
{
    public class ApplicationDbContext: IdentityDbContext
    {
         public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Rol> Rol { get; set; }
    }
}

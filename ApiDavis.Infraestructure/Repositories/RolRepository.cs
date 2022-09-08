using ApiDavis.Core.Entities;
using ApiDavis.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Infraestructure.Repositories
{
    public class RolRepository: IRolRepository
    {
        private readonly ApplicationDbContext _context;

        public RolRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<IEnumerable<Rol>> GetRoles()
        {
            try
            {
                var post = await _context.Rol.ToListAsync();
                return post;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

using DavisApi.Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DavisApi.Controllers
{
    [ApiController]
    [Route("api/Rol")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RolController: ControllerBase
    {
        private readonly ApplicationDbContext context;

        public RolController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Rol>>> Get()
        {
            return Ok(await context.Rol.ToListAsync());
        }

        [HttpGet("nombres")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Rol>>> GetByName(string descripcion)
        {
            return Ok(await context.Rol.Where(x => x.Descripcion== descripcion).ToListAsync());
        }
    }
}

using ApiDavis.Core.Entities;
using ApiDavis.Core.Interfaces;
using ApiDavis.Infraestructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiDavis.Controllers
{
    [Route("api/Rol")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RolController: ControllerBase
    {
        private readonly IRolRepository _repository;

        public RolController(IRolRepository repository )
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var roles = await _repository.GetRoles();
            return Ok(roles);
        }
    }
}

using ApiDavis.Core.DTOs;
using ApiDavis.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiDavis.Controllers
{
    [ApiController]
    [Route("api/Seguridad")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Seguridad: ControllerBase
    {
        private readonly ISeguridadRepository seguridadRepository;

        public Seguridad(ISeguridadRepository seguridadRepository)
        {
            this.seguridadRepository = seguridadRepository;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Autenticar(UsuarioLoginDTO usuario)
        {
            if(usuario == null)
            {
                return BadRequest();
            }
            var jwtToken = await seguridadRepository.Autenticar(usuario);

            if (!jwtToken.Estado)
            {
                var objeto = new
                {
                    mensaje = "Usuario Bloqueado"
                };
                return new OkObjectResult(new JsonResult(objeto));
            }
            if(jwtToken.AuthToken == null)
            {
                var objeto = new
                {
                    mensaje = "Datos Incorrectos"
                };
                return new OkObjectResult(new JsonResult(objeto));
                
            }
            return new OkObjectResult(new JsonResult(jwtToken));
        }
    }
}

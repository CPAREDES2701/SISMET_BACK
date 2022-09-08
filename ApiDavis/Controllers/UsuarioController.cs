using ApiDavis.Core.DTOs;
using ApiDavis.Core.Entities;
using ApiDavis.Core.Interfaces;
using ApiDavis.Core.Utilidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiDavis.Controllers
{
    [Route("api/Usuario")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuarioController: ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly HashService hashService;

        public UsuarioController(IUsuarioRepository usuarioRepository,HashService hashService)
        {
            _usuarioRepository = usuarioRepository;
            this.hashService = hashService;
        }
        [AllowAnonymous]
        [HttpPost("ObtenerUsuarios")]
        public async Task<ActionResult> GetUsuarios( PaginacionDTO paginacionDTO)
        {
            var usuarios = await _usuarioRepository.GetUsuarios(paginacionDTO);

            return Ok(usuarios);
        }

        [HttpGet("{id:int}", Name = "obtenerUsuario")]
        public async Task<ActionResult> GetUsuariosById(int id)
        {
            var usuarios = await _usuarioRepository.GetUsuarios(id);

            if (usuarios == null)
            {
                return NotFound("No existe el usuario");
            }

            return Ok(usuarios);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarUsuario([FromBody] UsuarioRequestDTO usuario, int id)
        {
            var data = await _usuarioRepository.ActualizarUsuario(usuario,id);
            
            if (data.existe==1)
            {
                var objeto = new
                {
                    mensaje = "El usuario a editar no existe",
                    estado = false
                };
                return new OkObjectResult(new JsonResult(objeto));
            }
            if (data.existe == 2)
            {
                var objeto = new
                {
                    mensaje = "Ya existe un usuario con esa información Username, correo, núero de documento",
                    estado = false
                };
                return new OkObjectResult(new JsonResult(objeto));
            }
            var objetoOk = new
            {
                mensaje = "El usuario ha sido actualizado correctamente",
                estado = true
            };
             return new OkObjectResult(new JsonResult(objetoOk));
        }
        [HttpPost("agregar")]
        public async Task<ActionResult> CrearUsuarioDTO(UsuarioRequestDTO usuario)
        {
            var data = await _usuarioRepository.CrearUsuario2(usuario);
            if (data)
            {
                return BadRequest("Ya existe un usuario registrado");
            }
            return Ok("Se registró el usuario correctamente");
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDTO>> EliminarUsuario(int id)
        {
            var resultado = await _usuarioRepository.EliminarUsuario(id);
            return Ok(resultado);
        }
        [AllowAnonymous]
        [HttpPost("CambiarContrasena")]
        public async Task<ActionResult> CambiarContraseña([FromBody] CambiarClaveDTO dato)
        {
            var resultado = await _usuarioRepository.CambiarContraseña(dato);

            return Ok(resultado);
        }
    }
}

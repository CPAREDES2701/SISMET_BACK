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
        //[HttpPost]
        //public async Task<ActionResult> CrearUsuario(Usuario usuario)
        //{
        //    var data =await _usuarioRepository.CrearUsuario(usuario);
        //    if (data)
        //    {
        //        return BadRequest("Ya existe un usuario registrado");
        //    }
        //    return Ok("Se registró el usuario correctamente");
        //}
        [HttpPost("agregar")]
        public async Task<ActionResult> CrearUsuarioDTO(UsuarioRequestDTO usuario)
        {
            var data = await _usuarioRepository.CrearUsuario2(usuario);
            if (data)
            {
                return BadRequest("Ya existe un usuario registrado");
            }
            return Ok("Se registrò el usuario correctamente");
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDTO>> EliminarUsuario(int id)
        {
            var resultado = await _usuarioRepository.EliminarUsuario(id);
            return Ok(resultado);
        }
       
    }
}

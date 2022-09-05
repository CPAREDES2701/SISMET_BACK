using ApiDavis.Core.DTOs;
using ApiDavis.Core.Entities;
using ApiDavis.Core.Interfaces;
using ApiDavis.Core.Utilidades;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Infraestructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly HashService hashService;
        

        public UsuarioRepository(ApplicationDbContext context, IMapper mapper, HashService hashService)
        {
            _context = context;
            _mapper = mapper;
            this.hashService = hashService;
        }

        public async Task<UsuarioExisteDTO> ActualizarUsuario(UsuarioRequestDTO usuario, int id)
        {
            UsuarioExisteDTO obj = new UsuarioExisteDTO();
            var existe = await _context.Usuario.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                obj.existe = 1;
                return obj;
            }
            else
            {
                var usuarioInfo = await _context.Usuario.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (usuarioInfo != null)
                {
                    usuarioInfo.Apellidos = usuario.Apellidos;
                    usuarioInfo.Nombres = usuario.Nombres;
                    usuarioInfo.NroDocumento = usuario.NroDocumento;
                    usuarioInfo.UserName = usuario.UserName;
                    usuarioInfo.Contrasena = hashService.Encriptar(usuario.Contrasena);
                    usuarioInfo.correo = usuario.correo;
                    usuarioInfo.TipoDocumento = usuario.TipoDocumento;
                    usuarioInfo.EmpresaId = usuario.EmpresaId;
                    usuarioInfo.RolId = usuario.RolId;
                    var existes = await _context.Usuario.AnyAsync(x => (x.UserName == usuarioInfo.UserName || x.NroDocumento == usuario.NroDocumento || x.correo == usuarioInfo.correo) && x.Id != usuarioInfo.Id);
                    if (existes)
                    {
                        obj.existe = 2;
                        return obj;
                    }
                    _context.Update(usuarioInfo);
                    await _context.SaveChangesAsync();
                   
                }
                obj.existe = 3;
                return obj;
            }
        }

        public async Task<bool> CrearUsuario(Usuario usuario)
        {
            var existeUsuario = await _context.Usuario.AnyAsync(p => p.UserName == usuario.UserName || p.correo == usuario.correo || p.NroDocumento == usuario.NroDocumento);
            if (existeUsuario)
            {
                return existeUsuario;
            }
            usuario.Contrasena = hashService.Encriptar(usuario.Contrasena);

            _context.Add(usuario);
            await _context.SaveChangesAsync();

            return existeUsuario;
        }

        public async Task<bool> CrearUsuario2(UsuarioRequestDTO usuario)
        {
            var existeUsuario = await _context.Usuario.AnyAsync(p => p.UserName == usuario.UserName || p.correo == usuario.correo || p.NroDocumento == usuario.NroDocumento);
            if (existeUsuario)
            {
                return existeUsuario;
            }
            usuario.Contrasena = hashService.Encriptar(usuario.Contrasena);

            var usuarios = _mapper.Map<Usuario>(usuario);
            usuarios.Intentos = 0;
            usuarios.Estado = true;
            usuarios.FechaCreacion = DateTime.Now;
            usuarios.FechaModificacion = DateTime.Now;
            _context.Add(usuarios);
            await _context.SaveChangesAsync();

            return existeUsuario;
        }

        public async Task<ResponseDTO> CambiarContraseña(string correo)
        {
            Constantes consta = new Constantes();
            var existe =await _context.Usuario.AnyAsync(u => u.correo==correo || u.UserName ==correo);
            ResponseDTO response = new ResponseDTO();

            if (!existe)
            {
                response.valid = false;
                response.message = "No existe el usuario";
                response.TypeError = "Warning";
                return response;
            }
            var usuarioDomain = await _context.Usuario.FirstOrDefaultAsync(x => x.correo == correo || x.UserName == correo);
            var contraseña = hashService.Desencriptar(usuarioDomain.Contrasena);
            RecuperarClaveEmail message = new RecuperarClaveEmail();

            message.CorreoCliente = usuarioDomain.correo;
            message.NombreCliente = usuarioDomain.Nombres + " " + usuarioDomain.Apellidos;
            message.Contrasena = contraseña;
            List<string> correos = new List<string>();
            correos.Add(message.CorreoCliente);

            var obj = new EmailData<RecuperarClaveEmail>
            {
                EmailType = 2,
                EmailList = correos,
                Model = message,
                HtmlTemplateName = Constantes.RecuperarContraseña
            };

            await hashService.EnviarCorreoAsync(obj,message);

            response.valid = true;
            response.message = $"Se ha enviado la contraseña satisfactoriamente";
            response.TypeError = "Success";
            return response;
        }
        
        public async Task<ResponseDTO> EliminarUsuario(int id)
        {
            var existe = await _context.Usuario.AnyAsync(p => p.Id == id);
            ResponseDTO response = new ResponseDTO();
            if (!existe)
            {
                response.valid = false;
                response.message = "No existe el usuario ha eliminar";
                response.TypeError = "Warning";
                return response;
            }
            
            var usuarioDomain = await _context.Usuario.FirstOrDefaultAsync(x => x.Id == id);
        
            if (usuarioDomain != null) {
                if (usuarioDomain.Estado == true)
                {
                    usuarioDomain.Estado = false;
                    response.message = "Se ha eliminado el usuario satisfactoriamente";
                }
                else
                {
                    usuarioDomain.Estado = true;
                    response.message = "Se ha habilitado el usuario satisfactoriamente";
                }
                
                var validateResult =  _context.Update(usuarioDomain);
                if (validateResult != null)
                {
                    await _context.SaveChangesAsync();
                    response.valid = true;
                    
                    response.TypeError = "Success";
                }

            }
            else
            {
                response.valid = false;
                response.message = "No existe el usuario ha eliminar";
                response.TypeError = "Warning";
                return response;
            }
       
            return response;
        }
        public async Task<ExportUsuarioDto> GetUsuarios(PaginacionDTO paginacionDTO)
        {
            ExportUsuarioDto objUsuarios = new ExportUsuarioDto();
            var queryable = _context.Usuario
                .Include(x => x.Rol)
                .Include(x => x.Empresa)
                .Where(x => paginacionDTO.correo != "" ? x.correo == paginacionDTO.correo : true)
                .Where(x => paginacionDTO.username != "" ? x.UserName == paginacionDTO.username : true)
                .Where(x => paginacionDTO.Nombres != "" ? x.Nombres == paginacionDTO.Nombres : true)
                .Where(x => paginacionDTO.Apellidos != "" ? x.Apellidos == paginacionDTO.Apellidos : true)
                .AsQueryable();
            double cantidad = await queryable.CountAsync();
            var usuarios = await queryable.OrderBy(usuario => usuario.Nombres).Paginar(paginacionDTO)
                                .ToListAsync();
            objUsuarios.cantidad = Convert.ToInt32(cantidad);
            objUsuarios.Usuarios = usuarios;
            return objUsuarios;
        }

       
        public async Task<Usuario> GetUsuarios(int id)
        {
            var existeUsuario = await _context.Usuario.Where(x => x.Id==id)
                .Include(x => x.Empresa)
                .ThenInclude(x=>x.Estacion)
                .Include(x => x.Rol)
                .FirstOrDefaultAsync();

            if(existeUsuario!= null)
            {
                existeUsuario.Contrasena = hashService.Desencriptar(existeUsuario.Contrasena);
            }
            
            return existeUsuario;

        }
    }
}

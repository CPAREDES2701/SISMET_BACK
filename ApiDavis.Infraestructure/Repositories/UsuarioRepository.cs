using ApiDavis.Core.DTOs;
using ApiDavis.Core.Entities;
using ApiDavis.Core.Interfaces;
using ApiDavis.Core.Utilidades;
using AutoMapper;
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

            _context.Add(usuarios);
            await _context.SaveChangesAsync();

            return existeUsuario;
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
            
            var usuarioDomain = await _context.Usuario.FirstOrDefaultAsync(x => x.Id == id && x.Estado == true);
        
            if (usuarioDomain != null) {
                usuarioDomain.Estado = false;
                var validateResult =  _context.Update(usuarioDomain);
                if (validateResult != null)
                {
                    await _context.SaveChangesAsync();
                    response.valid = true;
                    response.message = "Se ha eliminado el usuario satisfactoriamente";
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

        public async Task<IEnumerable<Usuario>> GetUsuarios()
        {
            var usuarios = await _context.Usuario.ToListAsync();
            return usuarios;
        }

        public async Task<Usuario> GetUsuarios(int id)
        {
            var existeUsuario = await _context.Usuario.FirstOrDefaultAsync(x => x.Id == id && x.Estado == true);
            
            return existeUsuario;

        }
    }
}

using ApiDavis.Core.DTOs;
using ApiDavis.Core.Interfaces;
using ApiDavis.Core.Utilidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Infraestructure.Repositories
{
    public class SeguridadRepository : ISeguridadRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly HashService hashService;

        public SeguridadRepository(ApplicationDbContext context,HashService hashService )
        {
            _context = context;
            this.hashService = hashService;
        }
        public async Task<JwtResponse> Autenticar(UsuarioLoginDTO usuario)
        {
            var encriptado = hashService.Encriptar(usuario.Password);
            var resultado = await _context.Usuario.AnyAsync(u => u.correo == usuario.usuario && u.Contrasena == encriptado);
            if (resultado)
            {
                return await hashService.ConstruirToken(usuario);
            }
            return new JwtResponse { };
        }
    }
}

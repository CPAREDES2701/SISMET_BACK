﻿using ApiDavis.Core.DTOs;
using ApiDavis.Core.Interfaces;
using ApiDavis.Core.Utilidades;
using Microsoft.AspNetCore.JsonPatch;
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
            try
            {
                hashService.log("---------------------------------------------");
                hashService.log("Inicio de loggin");
                hashService.log("---------------------------------------------");
                var encriptado = hashService.Encriptar(usuario.Password);

                var existe = await _context.Usuario.AnyAsync(u => u.UserName == usuario.usuario);

                if (!existe)
                {
                    return new JwtResponse
                    {
                        Message = "El Usuario no Existe"
                    };
                }

                var resultado = await _context.Usuario.Where(u => u.UserName == usuario.usuario && u.Contrasena == encriptado).FirstOrDefaultAsync();


                if (existe && resultado == null)
                {
                    var usuarioUpdate = await _context.Usuario.Where(u => u.UserName == usuario.usuario).FirstOrDefaultAsync();
                    if (usuarioUpdate.Estado == true)
                    {
                        if (usuarioUpdate.Intentos < 3)
                        {
                            if (usuarioUpdate.Intentos == 2)
                            {
                                usuarioUpdate.Estado = false;
                                _context.Update(usuarioUpdate);
                                await _context.SaveChangesAsync();
                            }
                            usuarioUpdate.Intentos = usuarioUpdate.Intentos + 1;
                            _context.Update(usuarioUpdate);
                            await _context.SaveChangesAsync();
                            return new JwtResponse
                            {
                                Message = "Datos Incorrectos"
                            };

                        }
                    }
                    else
                    {
                        return new JwtResponse
                        {
                            Message = "Usuario bloqueado"
                        };
                    }
                }


                if (resultado != null)
                {
                    if (resultado.Estado == false) return new JwtResponse { Message = "Usuario bloqueado" };
                    var usuarioToken = await _context.Usuario.Where(x => x.UserName == usuario.usuario).FirstOrDefaultAsync();
                    hashService.log("---------------------------------------------");
                    hashService.log("Fin de loggin");
                    hashService.log("---------------------------------------------");
                    return await hashService.ConstruirToken(usuarioToken);
                }
               
                return new JwtResponse { };
            }
            catch (Exception e)
            {
                hashService.log(e.Message);
                throw;
            }
        }

       
    }
}

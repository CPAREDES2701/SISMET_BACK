using ApiDavis.Core.DTOs;
using ApiDavis.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<ExportUsuarioDto> GetUsuarios(PaginacionDTO paginacionDTO);
        Task<Usuario> GetUsuarios(int id);
        Task<bool> CrearUsuario(Usuario usuario);
        Task<UsuarioExisteDTO> ActualizarUsuario(UsuarioRequestDTO usuario, int id);
        Task<bool> CrearUsuario2(UsuarioRequestDTO usuario);

        Task<ResponseDTO> EliminarUsuario(int id);
        Task<ResponseDTO> CambiarContraseña(CambiarClaveDTO correo);
    }


}

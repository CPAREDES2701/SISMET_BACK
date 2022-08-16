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
        Task<IEnumerable<Usuario>> GetUsuarios();
        Task<Usuario> GetUsuarios(int id);
        Task<bool> CrearUsuario(Usuario usuario);
        Task<bool> CrearUsuario2(UsuarioRequestDTO usuario);

        Task<ResponseDTO> EliminarUsuario(int id);
    }


}

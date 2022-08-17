using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class UsuarioResponseDTO
    {
        public string UserName { get; set; }
        
        public string Contrasena { get; set; }
    
        public string Nombres { get; set; }
       
        public string Apellidos { get; set; }
        
        public string TipoDocumento { get; set; }
        
        public string NroDocumento { get; set; }
        public string correo { get; set; }
    }
}

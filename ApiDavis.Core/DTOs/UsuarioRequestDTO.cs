using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class UsuarioRequestDTO
    {
       
        [Required]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }
        [Required] 
        public string Nombres { get; set; }
        [Required] 
        public string Apellidos { get; set; }
        [Required]
        public string TipoDocumento { get; set; }
        [Required]
        public string NroDocumento { get; set; }
        [EmailAddress]
        public string correo { get; set; }
       
       
    }
}

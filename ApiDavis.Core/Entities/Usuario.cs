using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Contrasena { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public bool Estado { get; set; }
        public int Intentos { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string correo { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
        public int RolId { get; set; }
        public Rol Rol { get; set; }

    }
}

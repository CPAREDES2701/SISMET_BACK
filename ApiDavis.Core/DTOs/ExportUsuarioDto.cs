using ApiDavis.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class ExportUsuarioDto
    {
        public int cantidad { get; set; }
        public List<Usuario> Usuarios { get; set; }
    }
}

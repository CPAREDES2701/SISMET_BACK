using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class RecuperarClaveEmail
    {
        public string NombreCliente { get; set; }
        public string CorreoCliente { get; set; }
        public string Contrasena { get; set; }
    }
}

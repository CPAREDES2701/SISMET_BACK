using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Entities
{
    public class Estacion
    {
        public int Id { get; set; }
        public string NombreEstacion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string Token { get; set; }
        public int EmpresaId { get; set; }
    }
}

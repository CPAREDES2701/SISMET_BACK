using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class EstacionResponseDTO
    {
        public int Id { get; set; }
        public string nombreEstacion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }
}

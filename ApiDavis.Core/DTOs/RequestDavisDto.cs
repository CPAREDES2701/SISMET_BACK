using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class RequestDavisDto
    {
        public int idSegundaEstacion { get; set; }
        public int idPrimeraEstacion { get; set; }
        public string fechaInicio  { get; set; }
        public string fechaFin { get; set; }
        public string horaInicio { get; set; }
        public string horaFin { get; set; }
    }
}

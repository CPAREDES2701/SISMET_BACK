using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class ListaReporte
    {
        public string fecha { get; set; }
        public List<ReporteInformacion> DetalleList { get; set; }
    }
}

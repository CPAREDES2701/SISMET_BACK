using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class RootDavisDTO
    {
        public bool existe { get; set; }
        //Punto de rocío
        public string dewpoint_c { get; set; }
        //presion barometrica
        public string pressure_mb { get; set; }
        public string pressure_string { get; set; }
        //Humedad
        public string relative_humidity { get; set; }
        //Temperatura
        public string temp_c { get; set; }
        public string temperature_string { get; set; }
        //Rosa vientos (direccion velocidad , angulos)
        public string wind_degrees { get; set; }
        public string wind_dir { get; set; }
        public string wind_kt { get; set; }
        public DavisCurrentDTO davis_current_observation { get; set; }
    }
}

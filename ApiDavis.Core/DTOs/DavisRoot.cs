using ApiDavis.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class DavisRoot
    {
        public string location { get; set; }
        public string dewpoint_c { get; set; }
        public string temp_c { get; set; }
        public string pressure_mb { get; set; }
        public string relative_humidity { get; set; }
        public string wind_degrees { get; set; }
        public string wind_dir { get; set; }
        public string wind_kt { get; set; }
        public DataDavis davis_current_observation { get; set; }
    }
}

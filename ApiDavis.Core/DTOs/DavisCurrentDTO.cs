using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class DavisCurrentDTO
    {
        //ET
        public string et_day { get; set; }
        public string et_month { get; set; }
        public string et_year { get; set; }
        //Luvia acumulada 
        public string rain_day_in { get; set; }
        public string rain_month_in { get; set; }
        public string rain_year_in { get; set; } 
        //Radiación solar  UW/m2
        public string solar_radiation { get; set; }
        //Temperatura Convertir a °C
        public string temp_day_high_f { get; set; }
        public string temp_day_high_time { get; set; }
        public string temp_day_low_f { get; set; }
        public string temp_day_low_time { get; set; }
        //Radiacion UV
        public string uv_index { get; set; }
        

    }
}

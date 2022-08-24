using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Entities
{
    public class DataDavis
    {
        public int Id { get; set; }     
        public string temp_day_high_f { get; set; }
        public string temp_day_high_time { get; set; }
        public string temp_day_low_f { get; set; }
        public string temp_day_low_time { get; set; }
        public string et_day { get; set; }
        public string et_month { get; set; }
        public string et_year { get; set; }
        public string rain_day_in { get; set; }
        public string rain_month_in { get; set; }
        public string rain_year_in { get; set; }
        public string solar_radiation { get; set; }

        public string uv_index { get; set; }
        public int EstacionId { get; set; }
    }
}

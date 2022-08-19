using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class RootDavisDTO
    {
        public string credit { get; set; }
        public string credit_URL { get; set; }
        public string disclaimer_url { get; set; }
        public string copyright_url { get; set; }
        public string privacy_policy_url { get; set; }
        public ImageDavisDTO image { get; set; }
        public string suggested_pickup { get; set; }
        public string suggested_pickup_period { get; set; }
        public string dewpoint_c { get; set; }
        public string dewpoint_f { get; set; }
        public string dewpoint_string { get; set; }
        public string heat_index_c { get; set; }
        public string heat_index_f { get; set; }
        public string heat_index_string { get; set; }
        public string location { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string observation_time { get; set; }
        public string observation_time_rfc822 { get; set; }
        public string pressure_in { get; set; }
        public string pressure_mb { get; set; }
        public string pressure_string { get; set; }
        public string relative_humidity { get; set; }
        public string station_id { get; set; }
        public string temp_c { get; set; }
        public string temp_f { get; set; }
        public string temperature_string { get; set; }
        public string wind_degrees { get; set; }
        public string wind_dir { get; set; }
        public string wind_kt { get; set; }
        public string wind_mph { get; set; }
        public string windchill_c { get; set; }
        public string windchill_f { get; set; }
        public string windchill_string { get; set; }
        public DavisCurrentDTO davis_current_observation { get; set; }
        public string time_to_generate { get; set; }
    }
}

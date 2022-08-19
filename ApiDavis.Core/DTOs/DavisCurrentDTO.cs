using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class DavisCurrentDTO
    {
        public string DID { get; set; }
        public string station_name { get; set; }
        public int observation_age { get; set; }
        public string dewpoint_day_high_f { get; set; }
        public string dewpoint_day_high_time { get; set; }
        public string dewpoint_day_low_f { get; set; }
        public string dewpoint_day_low_time { get; set; }
        public string dewpoint_month_high_f { get; set; }
        public string dewpoint_month_low_f { get; set; }
        public string dewpoint_year_high_f { get; set; }
        public string dewpoint_year_low_f { get; set; }
        public string et_day { get; set; }
        public string et_month { get; set; }
        public string et_year { get; set; }
        public string heat_index_day_high_f { get; set; }
        public string heat_index_day_high_time { get; set; }
        public string heat_index_month_high_f { get; set; }
        public string heat_index_year_high_f { get; set; }
        public string pressure_day_high_in { get; set; }
        public string pressure_day_high_time { get; set; }
        public string pressure_day_low_in { get; set; }
        public string pressure_day_low_time { get; set; }
        public string pressure_month_high_in { get; set; }
        public string pressure_month_low_in { get; set; }
        public string pressure_tendency_string { get; set; }
        public string pressure_year_high_in { get; set; }
        public string pressure_year_low_in { get; set; }
        public string rain_day_in { get; set; }
        public string rain_month_in { get; set; }
        public string rain_rate_day_high_in_per_hr { get; set; }
        public string rain_rate_hour_high_in_per_hr { get; set; }
        public string rain_rate_in_per_hr { get; set; }
        public string rain_rate_month_high_in_per_hr { get; set; }
        public string rain_rate_year_high_in_per_hr { get; set; }
        public string rain_storm_in { get; set; }
        public string rain_year_in { get; set; }
        public string relative_humidity_day_high { get; set; }
        public string relative_humidity_day_high_time { get; set; }
        public string relative_humidity_day_low { get; set; }
        public string relative_humidity_day_low_time { get; set; }
        public string relative_humidity_in { get; set; }
        public string relative_humidity_in_day_high { get; set; }
        public string relative_humidity_in_day_high_time { get; set; }
        public string relative_humidity_in_day_low { get; set; }
        public string relative_humidity_in_day_low_time { get; set; }
        public string relative_humidity_in_month_high { get; set; }
        public string relative_humidity_in_month_low { get; set; }
        public string relative_humidity_in_year_high { get; set; }
        public string relative_humidity_in_year_low { get; set; }
        public string solar_radiation { get; set; }
        public string solar_radiation_day_high { get; set; }
        public string solar_radiation_day_high_time { get; set; }
        public string solar_radiation_month_high { get; set; }
        public string solar_radiation_year_high { get; set; }
        public string sunrise { get; set; }
        public string sunset { get; set; }
        public string temp_day_high_f { get; set; }
        public string temp_day_high_time { get; set; }
        public string temp_day_low_f { get; set; }
        public string temp_day_low_time { get; set; }
        public string temp_month_high_f { get; set; }
        public string temp_month_low_f { get; set; }
        public string temp_year_high_f { get; set; }
        public string temp_year_low_f { get; set; }
        public string temp_in_day_high_f { get; set; }
        public string temp_in_day_high_time { get; set; }
        public string temp_in_day_low_f { get; set; }
        public string temp_in_day_low_time { get; set; }
        public string temp_in_f { get; set; }
        public string temp_in_month_high_f { get; set; }
        public string temp_in_month_low_f { get; set; }
        public string temp_in_year_high_f { get; set; }
        public string temp_in_year_low_f { get; set; }
        public string uv_index { get; set; }
        public string uv_index_day_high { get; set; }
        public string uv_index_day_high_time { get; set; }
        public string uv_index_month_high { get; set; }
        public string uv_index_year_high { get; set; }
        public string wind_day_high_mph { get; set; }
        public string wind_day_high_time { get; set; }
        public string wind_month_high_mph { get; set; }
        public string wind_ten_min_avg_mph { get; set; }
        public string wind_ten_min_gust_mph { get; set; }
        public string wind_year_high_mph { get; set; }
        public string windchill_day_low_f { get; set; }
        public string windchill_day_low_time { get; set; }
        public string windchill_month_low_f { get; set; }
        public string windchill_year_low_f { get; set; }
    }
}

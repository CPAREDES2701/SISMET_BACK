using ApiDavis.Core.DTOs;
using ApiDavis.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiDavis.Infraestructure.Repositories
{
    public class DavisRepository : IDavisRepository
    {
        private readonly ApplicationDbContext _context;

        public DavisRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<RootDavisDTO> Get(int id)
        {

            var existe = await _context.Estacion.Where(x => x.Id == id).FirstOrDefaultAsync();
            RootDavisDTO obj = new RootDavisDTO();
            if (existe == null)
            {
                obj.existe = true;
                return obj;
            }
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"https://api.weatherlink.com/v1/NoaaExt.json?user={existe.Usuario}&pass={existe.Clave}&apiToken={existe.Token}");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            RootDavisDTO root = JsonSerializer.Deserialize<RootDavisDTO>(responseBody);
            root.davis_current_observation.rain_day_in = Convert.ToString(Math.Round((Convert.ToDouble(root.davis_current_observation.rain_day_in) * 25.4),4));
            root.davis_current_observation.rain_month_in = Convert.ToString(Math.Round((Convert.ToDouble(root.davis_current_observation.rain_month_in) * 25.4),4));
            root.davis_current_observation.rain_year_in = Convert.ToString(Math.Round((Convert.ToDouble(root.davis_current_observation.rain_year_in) * 25.4),4));
            Console.WriteLine(root.davis_current_observation.temp_day_high_f);
            Console.WriteLine(root.davis_current_observation.temp_day_low_f);
            Console.WriteLine();
            root.davis_current_observation.temp_day_high_f = Convert.ToString(Math.Round((Convert.ToDouble(root.davis_current_observation.temp_day_high_f) - 32) * 5 / 9,4));
            root.davis_current_observation.temp_day_low_f = Convert.ToString(Math.Round((Convert.ToDouble(root.davis_current_observation.temp_day_low_f) - 32) * 5 / 9,4));
            
            return root;
        }
    }
}

using ApiDavis.Core.DTOs;
using ApiDavis.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiDavis.Infraestructure.Repositories
{
    public class HostedRepository : IHostedService, IDisposable
    {
        Timer _timer;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;

        public HostedRepository(IServiceScopeFactory factory)
        {
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            configuration = factory.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            _timer = new Timer(GuardarData, null , TimeSpan.Zero,TimeSpan.FromMinutes(1));
            //_timer = new Timer(data, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }
        public async void GuardarData(object state)
        {
            var data = await _context.Estacion.ToListAsync();
            string fecha = DateTime.Now.Minute.ToString();
            string fecha2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
     
            Console.WriteLine(fecha2);

            //if (fecha.EndsWith("15") || fecha.EndsWith("0") || fecha.EndsWith("30") || fecha.EndsWith("45"))
            //{
                //if (fecha.EndsWith("0"))
                //{
                //    fecha2 = Convert.ToDateTime(fecha2).AddMinutes(-1).ToString();
                //}
              
               
                List<ClientInfo> lista = new List<ClientInfo>();
                foreach (var item in data)
                {
                    ClientInfo client = new ClientInfo();
                    client.clave = item.Clave;
                    client.usuario = item.Usuario;
                    client.token = item.Token;
                    client.zona = item.EmpresaId;
                    lista.Add(client);
                }
                Stopwatch stopwatch = new Stopwatch();
                int contador = 0;
                stopwatch.Start();
                var opcionesParalelismo = new ParallelOptions { MaxDegreeOfParallelism = 5 };
                await Parallel.ForEachAsync(lista, opcionesParalelismo, async (listas, _) =>
                {
                    await TraerData(listas, fecha2);
                    contador++;
                });
                Console.WriteLine($"Tiempo Final:{stopwatch.Elapsed.Seconds.ToString()}");
                Console.WriteLine($"peticiones Final:{contador}");
            //}
        }
        public async void data(object obj)
        {
            await Informar();
        }
        public async Task Informar()
        {
            var datas = DateTime.Now.ToString("yyyy/MM-dd");
            var datass = await _context.Estacion.ToListAsync();
            var data = await _context.DataDavis.Where(x => x.EstacionId == 9 && x.fecha.ToString().Contains("2022-08-23")).ToListAsync();
            

            Console.WriteLine(data);
        }
        public async Task TraerData(ClientInfo objeto, string fecha)
        {
            string connStr = configuration.GetConnectionString("defaultConnection");
            string apiDavis = configuration.GetSection("ApiDavis").Value;
            MySqlConnection conn = new MySqlConnection(connStr);
            HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{apiDavis}?user={objeto.usuario}&pass={objeto.clave}&apiToken={objeto.token}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                DavisRoot root = JsonSerializer.Deserialize<DavisRoot>(responseBody);
                Console.WriteLine(root.dewpoint_c + "___" + root.location + "___" + objeto.zona);
                conn.Open();
                double temp_high = Convert.ToDouble(root.davis_current_observation.temp_day_high_f);
                temp_high = (temp_high - 32)*5/9;
                double temp_low = Convert.ToDouble(root.davis_current_observation.temp_day_low_f);
                temp_low = (temp_low - 32) * 5 / 9;
                root.davis_current_observation.temp_day_low_f =  Math.Round(temp_low,1).ToString();
                root.davis_current_observation.temp_day_high_f = Math.Round(temp_high, 1).ToString();
                string sql = $"INSERT INTO DataDavis (fecha,dewpoint_c,pressure_mb,relative_humidity,temp_c,temp_day_high_f,temp_day_high_time,temp_day_low_f,temp_day_low_time,wind_degrees,wind_dir,wind_kt,et_day,et_month,et_year,rain_day_in,rain_month_in,rain_year_in,solar_radiation,uv_index,EstacionId,observation_time)" +
                    $" VALUES ('{fecha}','{root.dewpoint_c}','{root.pressure_mb}','{root.relative_humidity}','{root.temp_c}','{root.davis_current_observation.temp_day_high_f}','{root.davis_current_observation.temp_day_high_time}','{root.davis_current_observation.temp_day_low_f}','{root.davis_current_observation.temp_day_low_time}','{root.wind_degrees}','{root.wind_dir}','{root.wind_kt}','{root.davis_current_observation.et_day}','{root.davis_current_observation.et_month}','{root.davis_current_observation.et_year}','{root.davis_current_observation.rain_day_in}','{root.davis_current_observation.rain_month_in}','{root.davis_current_observation.rain_year_in}','{root.davis_current_observation.solar_radiation}','{root.davis_current_observation.uv_index}','{objeto.zona}','{root.observation_time}')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

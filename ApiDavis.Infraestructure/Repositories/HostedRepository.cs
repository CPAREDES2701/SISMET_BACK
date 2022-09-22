using ApiDavis.Core.DTOs;
using ApiDavis.Core.Interfaces;
using ApiDavis.Core.Utilidades;
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
        Timer _timer2;
        private readonly ApplicationDbContext _context;
        private readonly ApplicationDbContext _context2;
        private readonly IConfiguration configuration;
        private readonly HashService hashService;

        public HostedRepository(IServiceScopeFactory factory, HashService hashService)
        {
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _context2 = factory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            configuration = factory.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();
            this.hashService = hashService;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            _timer = new Timer(GuardarData, null , TimeSpan.Zero,TimeSpan.FromMinutes(1));
            _timer2 = new Timer(data, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }
        public async void GuardarData(object state)
        {
            var data = await _context.Estacion.ToListAsync();
            string fecha = DateTime.Now.Minute.ToString();
            string fecha2 = DateTime.Now.AddSeconds(-DateTime.Now.Second).ToString("yyyy-MM-dd HH:mm:ss");
     
            Console.WriteLine(fecha2);

            if (fecha.EndsWith("15") || fecha.EndsWith("59") || fecha.EndsWith("30") || fecha.EndsWith("45"))
            {
                if (fecha.EndsWith("59"))
                {
                    fecha2= DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMinutes(+1).ToString("yyyy-MM-dd HH:mm:ss");
                }


                List<ClientInfo> lista = new List<ClientInfo>();
                foreach (var item in data)
                {
                    ClientInfo client = new ClientInfo();
                    client.clave = item.Clave;
                    client.usuario = item.Usuario;
                    client.token = item.Token;
                    client.zona = item.Id;
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
            }
        }
        public  void data(object obj)
        {
            string fecha = DateTime.Now.Minute.ToString();
            if (fecha.EndsWith("59"))
            {
                Informar();
            }
        }
        public async Task Informar()
        {
            var datas = DateTime.Now.ToString("yyyy-MM-dd");
            var fechaCorreo = DateTime.Now.ToString("dd-MM-yyyy");
            var estaciones =  _context2.Estacion.ToList();
            
            var correo = configuration.GetSection("DavisDestino").GetChildren().Select(c=> c.Value).ToArray();
           
            ListaReporte lista = new ListaReporte();
            List<ReporteInformacion> listaN = new List<ReporteInformacion>();
            for (int i = 0; i < estaciones.Count; i++)
            {
                var data = _context2.DataDavis.Where(x => x.EstacionId == estaciones.ElementAt(i).Id && x.fecha.ToString().Contains(datas)).ToList();
                if(data.Count != 96)
                {
                    ReporteInformacion message = new ReporteInformacion();
                    message.EstacionNombre = estaciones.ElementAt(i).NombreEstacion;
                    message.fecha = datas;
                    listaN.Add(message);
                   
                }
            }
            lista.fecha = fechaCorreo;
            lista.DetalleList = listaN;

            if (lista.DetalleList.Count>0)
            {
                List<string> correos = new List<string>();
                for (int i = 0; i < correo.Length; i++)
                {
                    correos.Add(correo.ElementAt(i));
                }

                string templateKey = "templateKey_Reporte";
                var obj = new EmailData<ListaReporte>
                {
                    EmailType = 2,
                    EmailList = correos,
                    Model = lista,
                    HtmlTemplateName = Constantes.Reporte
                };
                await hashService.EnviarCorreoReporteAsync(obj, lista, templateKey);
            }
            

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
                double wind_kt = Convert.ToDouble(root.wind_kt);
                root.wind_kh = Math.Round((wind_kt * 1.851),2).ToString();
                root.wind_ms = Math.Round((wind_kt / 1.944),2).ToString();
                root.davis_current_observation.rain_day_in = Math.Round(Convert.ToDouble(root.davis_current_observation.rain_day_in) * 25.4, 2).ToString(); ;
                root.davis_current_observation.rain_month_in = Math.Round(Convert.ToDouble(root.davis_current_observation.rain_month_in) * 25.4, 2).ToString();
                root.davis_current_observation.rain_year_in = Math.Round(Convert.ToDouble(root.davis_current_observation.rain_year_in) * 25.4, 2).ToString();
                string sql = $"INSERT INTO DataDavis (fecha,dewpoint_c,pressure_mb,relative_humidity,temp_c,temp_day_high_f,temp_day_high_time,temp_day_low_f,temp_day_low_time,wind_degrees,wind_dir,wind_kt,et_day,et_month,et_year,rain_day_in,rain_month_in,rain_year_in,solar_radiation,uv_index,EstacionId,observation_time,wind_ms,wind_kh)" +
                    $" VALUES ('{fecha}','{root.dewpoint_c}','{root.pressure_mb}','{root.relative_humidity}','{root.temp_c}','{root.davis_current_observation.temp_day_high_f}','{root.davis_current_observation.temp_day_high_time}','{root.davis_current_observation.temp_day_low_f}','{root.davis_current_observation.temp_day_low_time}','{root.wind_degrees}','{root.wind_dir}','{root.wind_kt}','{root.davis_current_observation.et_day}','{root.davis_current_observation.et_month}','{root.davis_current_observation.et_year}','{root.davis_current_observation.rain_day_in}','{root.davis_current_observation.rain_month_in}','{root.davis_current_observation.rain_year_in}','{root.davis_current_observation.solar_radiation}','{root.davis_current_observation.uv_index}','{objeto.zona}','{root.observation_time}','{root.wind_ms}','{root.wind_kh}')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                hashService.log("---------------------------------------------");
                hashService.log(ex.Message);
                hashService.log("---------------------------------------------");
                hashService.log(ex.StackTrace);
                hashService.log("---------------------------------------------");
                hashService.log(ex.InnerException.ToString());
                hashService.log("---------------------------------------------");
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

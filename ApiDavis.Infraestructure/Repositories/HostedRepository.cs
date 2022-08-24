using ApiDavis.Core.DTOs;
using ApiDavis.Core.Interfaces;
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
        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            _timer = new Timer(GuardarData, null , TimeSpan.Zero,TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }
        public async void GuardarData(object state)
        {
            string fecha = DateTime.Now.Minute.ToString();
            string fecha2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
            Console.WriteLine(fecha2);

            //if (fecha.EndsWith("15") || fecha.EndsWith("00") || fecha.EndsWith("30") || fecha.EndsWith("45"))
            //{

                List<ClientInfo> lista = new List<ClientInfo>()
            {
			    #region
			    new ClientInfo { usuario ="001D0A005187",clave="prolansac",token="47171655DAE64F9BB55E2CEBD2BB1C9D",zona=1},
                new ClientInfo { usuario ="001D0A00667E",clave="chiquerillo1",token="92DAC1EED3A04C3EBB05538C611508B6",zona=2},
                new ClientInfo { usuario ="001D0A00AD35",clave="12345678",token="86575624BB414E309475F1EB4D8521DE",zona=3},
                new ClientInfo { usuario ="001D0AE0861B",clave="Uchusquillo96",token="08BE9F1CE7CB41E9ADFE860C5C30C4E3",zona=4},
                new ClientInfo { usuario ="001D0A00981C",clave="natucultura2015",token="0E48FCA22D13422580EFF582D9CC655B",zona=5},
                new ClientInfo { usuario ="001D0A010E51",clave="natucultura2020",token="E9C4A274B8CE46C1A22C77DFEBC41F9C",zona=6},
                new ClientInfo { usuario ="001D0A0122AC",clave="natu1234$",token="4E90583454BB484AB910F63CEE59D8D7",zona=7},
                new ClientInfo { usuario ="001D0A0121A8",clave="natu1234$",token="4E90583454BB484AB910F63CEE59D8D7",zona=8},
                new ClientInfo { usuario ="001D0AE099B1",clave="Avoamerica2021...",token="BA723329BC6A4058B5B5CC781286FBC0",zona=9},
                new ClientInfo { usuario ="001D0AE0A32A",clave="pirona2022",token="9CF8849F930D4C3FBBDE0ACD357F1B31",zona=10},
                new ClientInfo { usuario ="001D0A01219D",clave="Bateas2020$$",token="C798ADC96C104586993C631ADF403EF9",zona=11},
                new ClientInfo { usuario ="001D0AE0A36E",clave="usuario12345",token="A31E5413C523479D8D2275A7241AB230",zona=12}
			    #endregion
		    };
                Stopwatch stopwatch = new Stopwatch();
                int contador = 0;
                stopwatch.Start();
                var opcionesParalelismo = new ParallelOptions { MaxDegreeOfParallelism = 10 };
                await Parallel.ForEachAsync(lista, opcionesParalelismo, async (listas, _) =>
                {
                    await TraerData(listas, fecha2);
                    contador++;
                });
                Console.WriteLine($"Tiempo Final:{stopwatch.Elapsed.Seconds.ToString()}");
                Console.WriteLine($"peticiones Final:{contador}");
            //}



        }
        public async Task TraerData(ClientInfo objeto, string fecha)
        {
            string connStr = "server=localhost; port=3306; database=davisbdprd; user=root; password=123456; Persist Security Info=False;";
            MySqlConnection conn = new MySqlConnection(connStr);
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"https://api.weatherlink.com/v1/NoaaExt.json?user={objeto.usuario}&pass={objeto.clave}&apiToken={objeto.token}");
            response.EnsureSuccessStatusCode();
           
            string responseBody = await response.Content.ReadAsStringAsync();
         
            DavisRoot root = JsonSerializer.Deserialize<DavisRoot>(responseBody);
            Console.WriteLine(root.dewpoint_c+"___"+root.location+"___"+objeto.zona);
            try
            {
                conn.Open();

                string sql = $"INSERT INTO DataDavis (fecha,dewpoint_c,pressure_mb,relative_humidity,temp_c,temp_day_high_f,temp_day_high_time,temp_day_low_f,temp_day_low_time,wind_degrees,wind_dir,wind_kt,et_day,et_month,et_year,rain_day_in,rain_month_in,rain_year_in,solar_radiation,uv_index,EstacionId)" +
                    $" VALUES ('{fecha}','{root.dewpoint_c}','{root.pressure_mb}','{root.relative_humidity}','{root.temp_c}','{root.davis_current_observation.temp_day_high_f}','{root.davis_current_observation.temp_day_high_time}','{root.davis_current_observation.temp_day_low_f}','{root.davis_current_observation.temp_day_low_time}','{root.wind_degrees}','{root.wind_dir}','{root.wind_kt}','{root.davis_current_observation.et_day}','{root.davis_current_observation.et_month}','{root.davis_current_observation.et_year}','{root.davis_current_observation.rain_day_in}','{root.davis_current_observation.rain_month_in}','{root.davis_current_observation.rain_year_in}','{root.davis_current_observation.solar_radiation}','{root.davis_current_observation.uv_index}','{objeto.zona}')";
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

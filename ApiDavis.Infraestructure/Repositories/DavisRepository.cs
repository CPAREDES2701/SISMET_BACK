using ApiDavis.Core.DTOs;
using ApiDavis.Core.Exceptions;
using ApiDavis.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //public async Task<ResponseDavisDto> GetEstacionByFecha(int idPrimeraEstacion, int idSegundaEstacion, DateTime fecha)
        public async Task<ResponseDavisDto> GetEstacionByFecha(RequestDavisDto dto)
        {
            ResponseDavisDto obj = new ResponseDavisDto();
            var existe =await _context.Estacion.AnyAsync(x => x.Id == dto.idPrimeraEstacion);
            var existeSecond = await _context.Estacion.AnyAsync(x => x.Id == dto.idSegundaEstacion);

            if (!existe && !existeSecond)
            {
                obj.message = $"No existe ninguna estación seleccionada";
                return obj;
            }
            if (!existe)
            {
                obj.message = $"No se encontró la estación {dto.idPrimeraEstacion} seleccionada";
                return obj;
            }
            if (!existeSecond)
            {
                obj.message = $"No se encontró la estación {dto.idSegundaEstacion} seleccionada";
                return obj;
            }
            
            string fechaInicio = dto.fechaInicio + " " + dto.horaInicio;
            string fechaFin = dto.fechaFin + " " + dto.horaFin;

            List<ResponseRootDavisDTO> lista = new List<ResponseRootDavisDTO>();

            
            if (dto.idPrimeraEstacion != 0)
            {
                var dataFirst = await _context.DataDavis.Where(x => x.EstacionId == dto.idPrimeraEstacion && (x.fecha >= Convert.ToDateTime(fechaInicio) && x.fecha <= Convert.ToDateTime(fechaFin))).ToListAsync();
                if (dataFirst.Count > 0)
                {
                    List<DataDavisEntiti> PrimeraEstacion = new List<DataDavisEntiti>();
                    foreach (var davis in dataFirst)
                    {
                        PrimeraEstacion.Add(davis);
                    }
                    obj.Estacion = PrimeraEstacion;

                }
            }
            if (dto.idSegundaEstacion != 0)
            {
                var dataSecond = await _context.DataDavis.Where(x => x.EstacionId == dto.idSegundaEstacion && (x.fecha >= Convert.ToDateTime(fechaInicio) && x.fecha <= Convert.ToDateTime(fechaFin))).ToListAsync();
                if (dataSecond.Count > 0)
                {
                    List<DataDavisEntiti> SegundaEstacion = new List<DataDavisEntiti>();
                    foreach (var davis in dataSecond)
                    {
                        SegundaEstacion.Add(davis);
                    }
                    obj.SecondEstacion = SegundaEstacion;

                }
            }
            return obj;
        }



    }
}

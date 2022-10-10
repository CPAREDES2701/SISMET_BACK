using ApiDavis.Core.DTOs;
using ApiDavis.Core.Interfaces;
using ApiDavis.Core.Utilidades;
using ClosedXML.Excel;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RazorEngine;
using System.Drawing;
using System.Text;

namespace ApiDavis.Controllers
{

    [ApiController]
    [Route("api/davis")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DavisController: ControllerBase
    {
        private readonly IDavisRepository _davisRepository;
        private readonly HashService _hashService;

        public DavisController(IDavisRepository davisRepository, HashService hashService)
        {
            _davisRepository = davisRepository;
            _hashService = hashService;
        }
        
        

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var resultado = await _davisRepository.Get(id);
            if(resultado.existe == true)
            {
                return NotFound("No existe estación a consultar");
            }
            return Ok(resultado);
        }

        //{
        //  "idSegundaEstacion": 1,
        //  "idPrimeraEstacion": 2,
        //  "fechaInicio": "2022-08-23",
        //  "fechaFin": "2022-08-23",
        //  "horaInicio": "12:15:05",
        //  "horaFin": "13:09:00"
        //}
        [HttpPost]
        //public async Task<ActionResult> GetEstacionByFecha(int idPrimeraEstacion, int idSegundaEstacion, DateTime fecha)
        public async Task<ActionResult> GetEstacionByFecha([FromBody]RequestDavisDto dto)
        {
            var resultado = await _davisRepository.GetEstacionByFecha(dto);
            if (resultado == null)
            {
                return NotFound("No existe estación a consultar");
            }
            return Ok(resultado);
        }
        [HttpPost("EstacionPaginado")]
        //public async Task<ActionResult> GetEstacionByFecha(int idPrimeraEstacion, int idSegundaEstacion, DateTime fecha)
        public async Task<ActionResult> GetEstacionByFechaPagination([FromBody] RequestDavisPaginadoDto dto)
        {
            var resultado = await _davisRepository.GetEstacionByFechaPagination(dto);
            if (resultado == null)
            {
                return NotFound("No existe estación a consultar");
            }
            return Ok(resultado);
        }
        [HttpPost("ExportarData")]
        //public async Task<ActionResult> GetEstacionByFecha(int idPrimeraEstacion, int idSegundaEstacion, DateTime fecha)
        public async Task<ActionResult> ExportarData([FromBody] RequestDavisDto dto)
        {
            var resultado = await _davisRepository.GetEstacionByFecha(dto);

            
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Información");
                    var currentRow = 1;
                    for(int i = 1; i <= 22; i++)
                    {
                        worksheet.Cell(currentRow, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#2ec6ff");
                        worksheet.Cell(currentRow, i).Style.Font.SetBold();
                        worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.White;
                    }
                    
                    worksheet.Cell(currentRow, 1).Value = "Fecha";
                    worksheet.Cell(currentRow, 2).Value = "Punto de rocío (ºC)";
                    worksheet.Cell(currentRow, 3).Value = "Presión (mb o hPa)";
                    worksheet.Cell(currentRow, 4).Value = "Humedad %";
                    worksheet.Cell(currentRow, 5).Value = "Temp. (ºC)";
                    worksheet.Cell(currentRow, 6).Value = "Temp. Máxima (°C)";
                    worksheet.Cell(currentRow, 7).Value = "Hora Temp. Máxima";
                    worksheet.Cell(currentRow, 8).Value = "Temp. Mínima (°C)";
                    worksheet.Cell(currentRow, 9).Value = "Hora Temp. Mínima";
                    worksheet.Cell(currentRow, 10).Value = "Rad Solar (W/m²)";
                    worksheet.Cell(currentRow, 11).Value = "Índice UV";
                    worksheet.Cell(currentRow, 12).Value = "Lluvia en el día (mm)";
                    worksheet.Cell(currentRow, 13).Value = "Lluvia en el mes (mm)";
                    worksheet.Cell(currentRow, 14).Value = "Lluvia en el año (mm)";
                    worksheet.Cell(currentRow, 15).Value = "ET en el día (mm)";
                    worksheet.Cell(currentRow, 16).Value = "ET en el mes (mm)";
                    worksheet.Cell(currentRow, 17).Value = "ET en el año (mm)";
                    worksheet.Cell(currentRow, 18).Value = "Dirección Viento (Grados)";
                    worksheet.Cell(currentRow, 19).Value = "Dirección Viento (Cardinales)";
                    worksheet.Cell(currentRow, 20).Value = "Velocidad del viento (kt)";
                    worksheet.Cell(currentRow, 21).Value = "Velocidad del viento (m/s)";
                    worksheet.Cell(currentRow, 22).Value = "Velocidad del viento (km/h)";
                    


                if (resultado.Estacion != null)
                    {
                    foreach (var item in resultado.Estacion)
                    {
                        currentRow++;
                        
                        worksheet.Cell(currentRow, 1).Style.DateFormat.Format = "dd/MM/yyyy hh:mm";
                        worksheet.Cell(currentRow, 1).Value = item.fecha;
                        worksheet.Cell(currentRow, 2).Value = item.dewpoint_c;
                        worksheet.Cell(currentRow, 3).Value = item.pressure_mb;
                        worksheet.Cell(currentRow, 4).Value = item.relative_humidity;
                        worksheet.Cell(currentRow, 5).Value = item.temp_c;
                        worksheet.Cell(currentRow, 6).Value = item.temp_day_high_f;
                        worksheet.Cell(currentRow, 7).Value = item.fecha.ToString().Split(" ")[0] + " "+item.temp_day_high_time;
                        worksheet.Cell(currentRow, 8).Value = item.temp_day_low_f;
                        worksheet.Cell(currentRow, 9).Value = item.fecha.ToString().Split(" ")[0] + " " + item.temp_day_low_time;
                        worksheet.Cell(currentRow, 10).Value = item.solar_radiation;
                        worksheet.Cell(currentRow, 11).Value = item.uv_index;
                        worksheet.Cell(currentRow, 12).Value = item.rain_day_in;
                        worksheet.Cell(currentRow, 13).Value = item.rain_month_in;
                        worksheet.Cell(currentRow, 14).Value = item.rain_year_in;
                        worksheet.Cell(currentRow, 15).Value = item.et_day;
                        worksheet.Cell(currentRow, 16).Value = item.et_month;
                        worksheet.Cell(currentRow, 17).Value = item.et_year;
                        worksheet.Cell(currentRow, 18).Value = item.wind_degrees;
                        worksheet.Cell(currentRow, 19).Value = _hashService.retornarDireccion(item.wind_dir);
                        worksheet.Cell(currentRow, 20).Value = item.wind_kt;
                        worksheet.Cell(currentRow, 21).Value = item.wind_ms;
                        worksheet.Cell(currentRow, 22).Value = item.wind_kh;


                    }
                    worksheet.Columns().AdjustToContents();
                    var worksheet2 = workbook.Worksheets.Add("Información2");
                    worksheet2.Columns().AdjustToContents();
                    var currentRow2 = 1;
                    for (int i = 1; i <= 22; i++)
                    {
                        worksheet2.Cell(currentRow2, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#2ec6ff");
                        worksheet.Cell(currentRow, i).Style.Font.SetBold();
                        worksheet2.Cell(currentRow2, i).Style.Font.FontColor = XLColor.White;
                    }

                    worksheet2.Cell(currentRow2, 1).Value = "Fecha";
                    worksheet2.Cell(currentRow2, 2).Value = "Punto de rocío (ºC)";
                    worksheet2.Cell(currentRow2, 3).Value = "Presión (mb o hPa)";
                    worksheet2.Cell(currentRow2, 4).Value = "Humedad %";
                    worksheet2.Cell(currentRow2, 5).Value = "Temp. (ºC)";
                    worksheet2.Cell(currentRow2, 6).Value = "Temp. Máxima (°C)";
                    worksheet2.Cell(currentRow2, 7).Value = "Hora Temp. Máxima";
                    worksheet2.Cell(currentRow2, 8).Value = "Temp. Mínima (°C)";
                    worksheet2.Cell(currentRow2, 9).Value = "Hora Temp. Mínima";
                    worksheet2.Cell(currentRow2, 10).Value = "Rad Solar (W/m²)";
                    worksheet2.Cell(currentRow2, 11).Value = "Índice UV";
                    worksheet2.Cell(currentRow2, 12).Value = "Lluvia en el día (mm)";
                    worksheet2.Cell(currentRow2, 13).Value = "Lluvia en el mes (mm)";
                    worksheet2.Cell(currentRow2, 14).Value = "Lluvia en el año (mm)";
                    worksheet2.Cell(currentRow2, 15).Value = "ET en el día (mm)";
                    worksheet2.Cell(currentRow2, 16).Value = "ET en el mes (mm)";
                    worksheet2.Cell(currentRow2, 17).Value = "ET en el año (mm)";
                    worksheet2.Cell(currentRow2, 18).Value = "Dirección Viento (Grados)";
                    worksheet2.Cell(currentRow2, 19).Value = "Dirección Viento (Cardinales)";
                    worksheet2.Cell(currentRow2, 20).Value = "Velocidad del viento (kt)";
                    worksheet2.Cell(currentRow2, 21).Value = "Velocidad del viento (m/s)";
                    worksheet2.Cell(currentRow2, 22).Value = "Velocidad del viento (km/h)";
                    foreach (var item in resultado.SecondEstacion)
                    {
                        currentRow2++;
                        worksheet2.Cell(currentRow2, 1).Value = item.fecha;
                        worksheet2.Cell(currentRow2, 2).Value = item.dewpoint_c;
                        worksheet2.Cell(currentRow2, 3).Value = item.pressure_mb;
                        worksheet2.Cell(currentRow2, 4).Value = item.relative_humidity;
                        worksheet2.Cell(currentRow2, 5).Value = item.temp_c;
                        worksheet2.Cell(currentRow2, 6).Value = item.temp_day_high_f;
                        worksheet2.Cell(currentRow2, 7).Value = item.fecha.ToString().Split(" ")[0] + " " + item.temp_day_high_time;
                        worksheet2.Cell(currentRow2, 8).Value = item.temp_day_low_f;
                        worksheet2.Cell(currentRow2, 9).Value = item.fecha.ToString().Split(" ")[0] + " " + item.temp_day_low_time;
                        worksheet2.Cell(currentRow2, 10).Value = item.solar_radiation;
                        worksheet2.Cell(currentRow2, 11).Value = item.uv_index;
                        worksheet2.Cell(currentRow2, 12).Value = item.rain_day_in;
                        worksheet2.Cell(currentRow2, 13).Value = item.rain_month_in;
                        worksheet2.Cell(currentRow2, 14).Value = item.rain_year_in;
                        worksheet2.Cell(currentRow2, 15).Value = item.et_day;
                        worksheet2.Cell(currentRow2, 16).Value = item.et_month;
                        worksheet2.Cell(currentRow2, 17).Value = item.et_year;
                        worksheet2.Cell(currentRow2, 18).Value = item.wind_degrees;
                        worksheet2.Cell(currentRow2, 19).Value = _hashService.retornarDireccion(item.wind_dir);
                        worksheet2.Cell(currentRow2, 20).Value = item.wind_kt;
                        worksheet2.Cell(currentRow2, 21).Value = item.wind_ms;
                        worksheet2.Cell(currentRow2, 22).Value = item.wind_kh;
                    }
                    worksheet2.Columns().AdjustToContents();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
                    }
                }
                else
                {
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
                    }
                }
                }
            
            

        }
        [HttpGet("GetEstaciones")]
        //public async Task<ActionResult> GetEstacionByFecha(int idPrimeraEstacion, int idSegundaEstacion, DateTime fecha)
        public async Task<ActionResult> GetEstaciones()
        {
            var resultado = await _davisRepository.GetEstaciones();
            if (resultado == null)
            {
                return NotFound("No existe estaciones a consultar");
            }
            return Ok(resultado);
        }

        [HttpPost("CrearEstacion")]
        //public async Task<ActionResult> GetEstacionByFecha(int idPrimeraEstacion, int idSegundaEstacion, DateTime fecha)
        public async Task<ActionResult> CrearEstacion([FromBody] EstacionRequestDTO estacion)
        {
            var resultado = await _davisRepository.CrearEstacion(estacion);
            if (resultado)
            {
                return BadRequest("Ya existe una estación registrada");
            }
            return Ok("Se registró la estación correctamente");
        }
        [HttpGet("{idEstacion:int}/{fechaInicio}/{fechaFin}")]
        public async Task<ActionResult<ResponseCalculoDTO>> GetHorasFrio(int idEstacion,string fechaInicio, string fechaFin)
        {
   
            var resultado = await _davisRepository.GetHorasFrio(idEstacion, fechaInicio,fechaFin);
          
            return Ok(resultado);
        }
        [HttpGet("RadiacionSolar/{idEstacion:int}/{fechaInicio}/{fechaFin}")]
        public async Task<ActionResult<ResponseCalculoDTO>> GetRadiacionSolar(int idEstacion, string fechaInicio, string fechaFin)
        {
         
            var resultado = await _davisRepository.GetRadiacionSolar(idEstacion, fechaInicio, fechaFin);
            
            return Ok(resultado);
        }
        [HttpGet("InduccionFloral/{idEstacion:int}")] 
        public async Task<ActionResult<List<InduccionFloral>>> GetInduccionFloral(int idEstacion)
        {
            var resultado = await _davisRepository.GetInduccionFloral2(idEstacion);  
            return Ok(resultado);
        }
    }
}

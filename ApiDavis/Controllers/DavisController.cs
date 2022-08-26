using ApiDavis.Core.DTOs;
using ApiDavis.Core.Interfaces;
using ClosedXML.Excel;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

using RazorEngine;
using System.Drawing;
using System.Text;

namespace ApiDavis.Controllers
{

    [ApiController]
    [Route("api/davis")]
    public class DavisController: ControllerBase
    {
        private readonly IDavisRepository _davisRepository;
        

        public DavisController(IDavisRepository davisRepository)
        {
            _davisRepository = davisRepository;
            
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
        [HttpPost("ExportarData")]
        //public async Task<ActionResult> GetEstacionByFecha(int idPrimeraEstacion, int idSegundaEstacion, DateTime fecha)
        public async Task<ActionResult> ExportarData([FromBody] RequestDavisDto dto)
        {
            var resultado = await _davisRepository.GetEstacionByFecha(dto);

            
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Información");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Id";
                    worksheet.Cell(currentRow, 2).Value = "Data";
                    if (resultado.Estacion != null)
                    {
                    foreach (var item in resultado.Estacion)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.temp_day_high_time;
                        worksheet.Cell(currentRow, 2).Value = item.temp_day_high_f;
                    }
                    var worksheet2 = workbook.Worksheets.Add("Información2");
                    var currentRow2 = 1;
                    worksheet2.Cell(currentRow2, 1).Value = "Id";
                    worksheet2.Cell(currentRow2, 2).Value = "Data";
                    foreach (var item in resultado.Estacion)
                    {
                        currentRow2++;
                        worksheet2.Cell(currentRow2, 1).Value = item.temp_day_high_time;
                        worksheet2.Cell(currentRow2, 2).Value = item.temp_day_high_f;
                    }
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

    }
}

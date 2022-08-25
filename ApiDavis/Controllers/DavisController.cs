using ApiDavis.Core.DTOs;
using ApiDavis.Core.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
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

            var builder = new StringBuilder();
            builder.AppendLine("ID,Username");
            foreach (var item in resultado.Estacion)
            {
                builder.AppendLine($"{item.et_day},{item.temp_c}");
            }
            return File(System.Text.Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "users.csv");

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

    }
}

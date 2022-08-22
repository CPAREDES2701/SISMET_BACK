using ApiDavis.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        //Tiempo Real Api
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
    }
}

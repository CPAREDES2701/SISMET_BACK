using ApiDavis.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiDavis.Controllers
{
    [ApiController]
    [Route("api/Empresa")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmpresaController: ControllerBase
    {
        private readonly IEmpresaRepository empresaRepository;

        public EmpresaController(IEmpresaRepository empresaRepository)
        {
            this.empresaRepository = empresaRepository;
        }
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var resultados = await empresaRepository.Get();
            return Ok(resultados);
        }
    }
}

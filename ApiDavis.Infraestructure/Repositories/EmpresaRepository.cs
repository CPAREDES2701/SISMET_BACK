using ApiDavis.Core.DTOs;
using ApiDavis.Core.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Infraestructure.Repositories
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public EmpresaRepository(ApplicationDbContext context, IMapper mapper)
        {
            this._context = context;
            this.mapper = mapper;
        }
        public async Task<List<EmpresaResponseDTO>> Get()
        {
            var empresas = await _context.Empresa.ToListAsync();
            var empresasResponse = mapper.Map<List<EmpresaResponseDTO>>(empresas);
            return empresasResponse;
        }
    }
}

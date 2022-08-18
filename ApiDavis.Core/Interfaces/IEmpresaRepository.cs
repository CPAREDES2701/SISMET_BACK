using ApiDavis.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Interfaces
{
    public interface IEmpresaRepository
    {
        Task<List<EmpresaResponseDTO>> Get();
    }
}

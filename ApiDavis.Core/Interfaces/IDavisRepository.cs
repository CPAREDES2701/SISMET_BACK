using ApiDavis.Core.DTOs;
using ApiDavis.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Interfaces
{
    public interface IDavisRepository
    {
        Task<RootDavisDTO> Get(int id);
        Task<ResponseDavisDto> GetEstacionByFecha(RequestDavisDto obj);
        Task<IEnumerable<EstacionResponseDTO>> GetEstaciones();
        Task<bool> CrearEstacion(EstacionRequestDTO estacion);
        Task<ResponseCalculoDTO> GetHorasFrio(int idEstacion,string fechaInicio, string fechaFin);
        Task<string> GetRadiacionSolar(int idEstacion, string fechaInicio, string fechaFin);
    }
}

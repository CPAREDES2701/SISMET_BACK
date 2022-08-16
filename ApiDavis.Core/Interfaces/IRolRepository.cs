using ApiDavis.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Interfaces
{
    public interface IRolRepository
    {
        Task<IEnumerable<Rol>> GetRoles();
    }
}

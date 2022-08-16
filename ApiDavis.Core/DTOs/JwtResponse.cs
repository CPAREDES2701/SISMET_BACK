using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class JwtResponse
    {
        public string AuthToken { get; set; }
        public DateTime ExpireIn { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class ResponseDTO
    {
        public bool valid { get; set; }
        public string message { get; set; }
        public string TypeError { get; set; }
    }
}

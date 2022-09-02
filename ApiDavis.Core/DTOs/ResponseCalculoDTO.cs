using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class ResponseCalculoDTO
    {
        public List<HistogramTable> HistogramTable { get; set; }
        public string valor { get; set; }
        public bool valid { get; set; }
    }
}

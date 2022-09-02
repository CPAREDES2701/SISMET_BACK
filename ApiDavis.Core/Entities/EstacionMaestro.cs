using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Entities
{
    public class EstacionMaestro
    {
        public int Id { get; set; }
        public int temperatura { get; set; }
        public int mesInicio { get; set; }
        public int mesFin { get; set; }
        public int EstacionId { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Exceptions
{
    public class ExceptionManager : Exception
    {
        public ExceptionManager(string message)
            : base(message)
        {
        }
    }
}

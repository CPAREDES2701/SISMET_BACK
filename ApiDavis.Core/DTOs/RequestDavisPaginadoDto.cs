using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.DTOs
{
    public class RequestDavisPaginadoDto
    {
        public int idSegundaEstacion { get; set; }
        public int idPrimeraEstacion { get; set; }
        public string fechaInicio { get; set; }
        public string fechaFin { get; set; }
        public string horaInicio { get; set; }
        public string horaFin { get; set; }
        public int Pagina { get; set; }
        private int recordsPorPagina = 10;
        private readonly int cantidadMaximaPorPagina = 50;

        public int RecordsPorPagina
        {
            get { return recordsPorPagina; }
            set
            {
                recordsPorPagina = (value > cantidadMaximaPorPagina) ? cantidadMaximaPorPagina : value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Utilidades
{
    public class Constantes
    {
        public const string PathFinanciamientoTemplate = "\\\\Content\\";
        public const string RecuperarContraseña = "\\RecuperarContrasena.html";
        public const string CrearUsuario = "\\RegistroUsuario.html";
        public const string Reporte = "\\Reporte.html";
        public const string AgostoIni = "-08-01";
        public const string AgostoFin = "-08-31";
        public const string SetiembreIni = "-09-01";
        public const string SetiembreFin = "-09-30";
        public const string OctubreIni = "-10-01";
        public const string OctubreFin = "-10-31";
        public const string NoviembreIni = "-11-01";
        public const string NoviembreFin = "-11-30";
        public static readonly string[] meses = { "Enero","Febrero","Marzo","Abril","Mayo", "Junio", "Julio", "Agosto", "Setiembre", "Octubre", "Noviembre", "Diciembre" };


        public static DateTime retornarFechaIni(int number)
        {
            DateTime retornarFecha = new DateTime();
            var anio = DateTime.Now.Year.ToString();
            if (number == 8)
            {
                retornarFecha = Convert.ToDateTime(anio + AgostoIni);
            }
            if (number == 9)
            {
                retornarFecha = Convert.ToDateTime(anio + SetiembreIni);
            }
            if (number == 10)
            {
                retornarFecha = Convert.ToDateTime(anio + OctubreIni);
            }
            if (number == 11)
            {
                retornarFecha = Convert.ToDateTime(anio + NoviembreIni);
            }
            return retornarFecha;
        }
        public static DateTime retornarFechaFin(int number)
        {
            DateTime retornarFecha = new DateTime();
            var anio = DateTime.Now.Year.ToString();
            if (number == 8)
            {
                retornarFecha = Convert.ToDateTime(anio + AgostoFin);
            }
            if (number == 9)
            {
                retornarFecha = Convert.ToDateTime(anio + SetiembreFin);
            }
            if (number == 10)
            {
                retornarFecha = Convert.ToDateTime(anio + OctubreFin);
            }
            if (number == 11)
            {
                retornarFecha = Convert.ToDateTime(anio + NoviembreIni);
            }
            return retornarFecha;
        }
    }
}

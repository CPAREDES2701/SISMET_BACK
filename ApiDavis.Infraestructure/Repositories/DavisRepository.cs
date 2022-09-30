using ApiDavis.Core.DTOs;
using ApiDavis.Core.Entities;
using ApiDavis.Core.Exceptions;
using ApiDavis.Core.Interfaces;
using ApiDavis.Core.Utilidades;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiDavis.Infraestructure.Repositories
{
    public class DavisRepository : IDavisRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly HashService hashService;

        public DavisRepository(ApplicationDbContext context, IMapper mapper,HashService hashService)
        {
            _context = context;
            this.mapper = mapper;
            this.hashService = hashService;
        }

        public async Task<List<InduccionFloral>> GetInduccionFloral2(int idEstacion)
        {
            List<InduccionFloral> induccion = new List<InduccionFloral>();
            EstacionMaestro obj = new EstacionMaestro();
             var estacionMaestra = await _context.EstacionMaestro.Where(x => x.EstacionId == idEstacion).ToListAsync();
                obj.mesInicio = estacionMaestra.ElementAt(0).mesInicio;
                obj.mesFin = estacionMaestra.ElementAt(0).mesFin;
                obj.temperatura = estacionMaestra.ElementAt(0).temperatura;

            var connectionString = hashService.conection();
            

            for (int j = obj.mesInicio; j <= obj.mesFin; j++)
            {
                InduccionFloral objetoMes = new InduccionFloral();
                var anio = DateTime.Now.Year.ToString();


                string fechaIni = Constantes.retornarFechaIni(j);
                string fechaFin = Constantes.retornarFechaFin(j);
                using (var connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = @"with  primera as(
	                select CONVERT(fecha,char(10)) as fecha, substring(CONVERT(fecha,char(20)),12,8)  as hora, temp_day_high_F ,temp_day_low_f 
	                from datadavis where Date(fecha) between '" + fechaIni + @"' and '" + fechaFin + @"'  AND estacionId ="+idEstacion+ @"),
	                segunda as(
	                SELECT MAX(a.FECHA) AS FECHA , MAX(b.hora) AS HORA,round(sum(a.temp_day_high_F)/count(1),3) as temp_max,round(sum(a.temp_day_high_F)/count(1),3) as temp_min
	                from primera a left join grupotiempo b
	                on a.hora=b.hora
	                group by fecha,grupo
	                ), tercera as(
                    SELECT FECHA,HORA, CASE WHEN ((TEMP_MAX+TEMP_MIN)/2)-10 >0 THEN ((TEMP_MAX+TEMP_MIN)/2)-10 ELSE 0 END as suma FROM SEGUNDA
                    ) SELECT ROUND(SUM(SUMA)/COUNT(SUMA),2) AS TOTAL fROM TERCERA";
                    connection.Open();
                    MySqlDataReader result = command.ExecuteReader();
                    string resultado="";
                    while (result.Read())
                    {
                        resultado = result["TOTAL"].ToString();
                        objetoMes.mes = Constantes.meses[j - 1];
                        objetoMes.valor = resultado == "" ? 0 : Convert.ToDouble(resultado);

                        induccion.Add(objetoMes);
                    }

                    result.Close();
                }
            }
            return induccion;
        }
        public async Task<List<InduccionFloral>> GetInduccionFloral(int idEstacion)
        {
            try
            {
                List<InduccionFloral> induccion = new List<InduccionFloral>();
                ResponseCalculoDTO responseCalculo = new ResponseCalculoDTO();

                EstacionMaestro obj = new EstacionMaestro();
                var estacionMaestra = await _context.EstacionMaestro.Where(x => x.EstacionId == idEstacion).ToListAsync();
                obj.mesInicio = estacionMaestra.ElementAt(0).mesInicio;
                obj.mesFin = estacionMaestra.ElementAt(0).mesFin;
                obj.temperatura = estacionMaestra.ElementAt(0).temperatura;
                for (int j = obj.mesInicio; j <= obj.mesFin; j++)
                {
                    InduccionFloral objetoMes = new InduccionFloral();
                    var anio = DateTime.Now.Year.ToString();
                    DateTime agostoIni = Convert.ToDateTime(anio + Constantes.AgostoIni);
                    DateTime agostoFin = Convert.ToDateTime(anio + Constantes.AgostoFin);

                    //agostoIni = Constantes.retornarFechaIni(j);
                    //agostoFin = Constantes.retornarFechaFin(j);

                    agostoFin = agostoFin.AddDays(1).AddSeconds(-1);
                    var dataAgruppada = await _context.DataDavis.Where(x => x.EstacionId == idEstacion && (x.fecha >= agostoIni && x.fecha <= agostoFin.AddDays(1).AddSeconds(-1))).ToListAsync();

                    double suma = 0;
                    for (int i = 0; i < dataAgruppada.Count(); i++)
                    {
                        double tempHigh = Convert.ToDouble(dataAgruppada.ElementAt(i).temp_day_high_f);
                        double tempLow = Convert.ToDouble(dataAgruppada.ElementAt(i).temp_day_low_f);
                        double resultado = ((tempHigh + tempLow) / 2) - obj.temperatura;
                        if (resultado > 0)
                        {
                            suma = suma + resultado;
                        }
                    }
                    double total = 0;
                    if (suma > 0)
                    {
                        total = suma / dataAgruppada.Count();
                    }
                    else
                    {
                        total = 0;
                    }


                    objetoMes.mes = Constantes.meses[j - 1];
                    objetoMes.valor = Math.Round(total, 2);

                    induccion.Add(objetoMes);
                }

                return induccion;
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        public async Task<ResponseCalculoDTO> GetRadiacionSolar(int idEstacion, string fechaInicio, string fechaFin)
        {
            try
            {
                ResponseCalculoDTO responseCalculo = new ResponseCalculoDTO();
                var fechaI = Convert.ToDateTime(fechaInicio);
                var fechaF = Convert.ToDateTime(fechaFin);
                fechaF = fechaF.AddDays(1).AddSeconds(-1);
                var data = await _context.DataDavis.Where(x => x.EstacionId == idEstacion && (x.fecha >= Convert.ToDateTime(fechaInicio) && x.fecha <= Convert.ToDateTime(fechaFin))).ToListAsync();

                var dates = new List<DateTime>();
                var arregloDates = new List<string>();
                for (var dt = fechaI; dt <= fechaF; dt = dt.AddDays(1))
                {
                    dates.Add(dt);
                    arregloDates.Add(dt.Date.ToShortDateString());
                }

                var dataAgrupada = await _context.DataDavis.Where(x => x.EstacionId == idEstacion && (x.fecha >= fechaI && x.fecha <= fechaF)).ToListAsync();
                if (dataAgrupada.Count() > 0)
                {
                    var sorted = dataAgrupada.OrderByDescending(da => da.fecha).ToArray();

                    var objecto = sorted.Where(x => Convert.ToDouble(x.solar_radiation) > 0).GroupBy(x => x.fecha.Date).Select(g => g.ToList()).ToList();

                    var histograma = new List<HistogramTable>();

                    var fechas = objecto.ElementAt(0).ElementAt(0).fecha;
                    for (int i = 0; i < objecto.Count(); i++)
                    {
                        for (int j = 0; j < 1; j++)
                        {
                            HistogramTable objeto = new HistogramTable();
                            objeto.fecha = objecto.ElementAt(i).ElementAt(j).TheDate.ToString();
                            objeto.horas = ((float)objecto.ElementAt(i).Count()) / (float)4;
                            histograma.Add(objeto);
                        }
                    }
                    for (int i = 0; i < arregloDates.Count(); i++)
                    {
                        string fecha = arregloDates.ElementAt(i);

                        if (!histograma.Any(k => k.fecha == fecha))
                        {
                            HistogramTable objeto = new HistogramTable();
                            objeto.fecha = fecha;
                            objeto.horas = 0;
                            histograma.Add(objeto);
                        }
                    }
                    int acumula = 0;
                    foreach (var item in data)
                    {
                        acumula = Convert.ToDouble(item.solar_radiation) > 0 ? acumula + 1 : acumula;
                    }

                    float tiempo = (float)acumula / (float)4;
                    int valor = Convert.ToInt32(Math.Floor(tiempo));
                    string minutes = (tiempo - valor) == 0.75 ? "45" : (tiempo - valor) == 0.50 ? "30" : (tiempo - valor) == 0.25 ? "15" : "0";
                    var histoFinal = histograma.OrderBy(da => da.fecha).ToArray();
                    List<HistogramTable> lista = new List<HistogramTable>();
                    for (int i = 0; i < histoFinal.Count(); i++)
                    {
                        HistogramTable objeto = new HistogramTable();
                        objeto.fecha = histoFinal.ElementAt(i).fecha;
                        objeto.horas = histoFinal.ElementAt(i).horas;
                        lista.Add(objeto);
                    }
                    responseCalculo.valor = valor.ToString() + " Hr y " + minutes + " Min";
                    responseCalculo.valid = true;
                    responseCalculo.HistogramTable = lista;
                    return responseCalculo;

                }
                else
                {
                    var histograma = new List<HistogramTable>();
                    for (int i = 0; i < arregloDates.Count(); i++)
                    {
                        string fecha = arregloDates.ElementAt(i);
                        HistogramTable objeto = new HistogramTable();
                        objeto.fecha = fecha;
                        objeto.horas = 0;
                        histograma.Add(objeto);

                    }
                    responseCalculo.HistogramTable = histograma;
                    responseCalculo.valor = "No se registraron horas de radiación solar"; ;
                    responseCalculo.valid = true;
                    return responseCalculo;
                }

            }
            catch (Exception)
            {

                throw;
            }
           
            

        }
        public async Task<ResponseCalculoDTO> GetHorasFrio(int idEstacion,string fechaInicio, string fechaFin)
        {
            try
            {
                ResponseCalculoDTO responseCalculo = new ResponseCalculoDTO();
                var fechaI = Convert.ToDateTime(fechaInicio);
                var fechaF = Convert.ToDateTime(fechaFin);
                fechaF = fechaF.AddDays(1).AddSeconds(-1);
                var data = await _context.DataDavis.Where(x => x.EstacionId == idEstacion && (x.fecha >= fechaI && x.fecha <= fechaF)).ToListAsync();
                var dates = new List<DateTime>();
                var arregloDates = new List<string>();
                for (var dt = fechaI; dt <= fechaF; dt = dt.AddDays(1))
                {
                    dates.Add(dt);
                    arregloDates.Add(dt.Date.ToShortDateString());
                }

                var dataAgrupada = await _context.DataDavis.Where(x => x.EstacionId == idEstacion && (x.fecha >= fechaI && x.fecha <= fechaF)).ToListAsync();
                if (dataAgrupada.Count() > 0)
                {
                    var sorted = dataAgrupada.OrderByDescending(da => da.fecha).ToArray();

                    var objecto = sorted.Where(x => Convert.ToDouble(x.temp_c) <= 7).GroupBy(x => x.fecha.Date).Select(g => g.ToList()).ToList();

                    if(objecto.Count ==0){
                        
                            var histogramas = new List<HistogramTable>();
                            for (int i = 0; i < arregloDates.Count(); i++)
                            {
                                string fecha = arregloDates.ElementAt(i);
                                HistogramTable objeto = new HistogramTable();
                                objeto.fecha = fecha;
                                objeto.horas = 0;
                                histogramas.Add(objeto);

                            }
                            responseCalculo.HistogramTable = histogramas;
                            responseCalculo.valor = "No se registraron horas frío"; ;
                            responseCalculo.valid = true;
                            return responseCalculo;
                       
                    }
                    var histograma = new List<HistogramTable>();

                    var fechas = objecto.ElementAt(0).ElementAt(0).fecha;

                    for (int i = 0; i < objecto.Count(); i++)
                    {
                        for (int j = 0; j < 1; j++)
                        {
                            HistogramTable objeto = new HistogramTable();
                            objeto.fecha = objecto.ElementAt(i).ElementAt(j).TheDate.ToString();
                            objeto.horas = ((float)objecto.ElementAt(i).Count()) / (float)4;
                            histograma.Add(objeto);
                        }
                    }


                    for (int i = 0; i < arregloDates.Count(); i++)
                    {
                        string fecha = arregloDates.ElementAt(i);

                        if (!histograma.Any(k => k.fecha == fecha))
                        {
                            HistogramTable objeto = new HistogramTable();
                            objeto.fecha = fecha;
                            objeto.horas = 0;
                            histograma.Add(objeto);
                        }
                    }

                    int acumula = 0;
                    foreach (var item in data)
                    {
                        acumula = Convert.ToDouble(item.temp_c) <= 20 ? acumula + 1 : acumula;
                    }


                    float tiempo = (float)acumula / (float)4;
                    int valor = Convert.ToInt32(Math.Floor(tiempo));
                    string minutes = (tiempo - valor) == 0.75 ? "45" : (tiempo - valor) == 0.50 ? "30" : (tiempo - valor) == 0.25 ? "15" : "0";
                    var histoFinal = histograma.OrderBy(da => da.fecha).ToArray();
                    List<HistogramTable> lista = new List<HistogramTable>();
                    for (int i = 0; i < histoFinal.Count(); i++)
                    {
                        HistogramTable objeto = new HistogramTable();
                        objeto.fecha = histoFinal.ElementAt(i).fecha;
                        objeto.horas = histoFinal.ElementAt(i).horas;
                        lista.Add(objeto);
                    }
                    responseCalculo.valor = valor.ToString() + " Hr y " + minutes + " Min";
                    responseCalculo.valid = true;
                    responseCalculo.HistogramTable = lista;
                    return responseCalculo;

                }
                else
                {
                    var histograma = new List<HistogramTable>();
                    for (int i = 0; i < arregloDates.Count(); i++)
                    {
                        string fecha = arregloDates.ElementAt(i);
                        HistogramTable objeto = new HistogramTable();
                        objeto.fecha = fecha;
                        objeto.horas = 0;
                        histograma.Add(objeto);

                    }
                    responseCalculo.HistogramTable = histograma;
                    responseCalculo.valor = "No se registraron horas frío"; ;
                    responseCalculo.valid = true;
                    return responseCalculo;
                }
            }
            catch (Exception)
            {

                throw;
            }
        
        }
        public async Task<RootDavisDTO> Get(int id)
        {

            try
            {
                var existe = await _context.Estacion.Where(x => x.Id == id).FirstOrDefaultAsync();
                RootDavisDTO obj = new RootDavisDTO();
                if (existe == null)
                {
                    obj.existe = true;
                    return obj;
                }
                HttpClient client = new HttpClient();
                
                HttpResponseMessage response = await client.GetAsync($"https://api.weatherlink.com/v1/NoaaExt.json?user={existe.Usuario}&pass={existe.Clave}&apiToken={existe.Token}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                RootDavisDTO root = JsonSerializer.Deserialize<RootDavisDTO>(responseBody);
                root.davis_current_observation.rain_day_in = Convert.ToString(Math.Round((Convert.ToDouble(root.davis_current_observation.rain_day_in) * 25.4), 4));
                root.davis_current_observation.rain_month_in = Convert.ToString(Math.Round((Convert.ToDouble(root.davis_current_observation.rain_month_in) * 25.4), 4));
                root.davis_current_observation.rain_year_in = Convert.ToString(Math.Round((Convert.ToDouble(root.davis_current_observation.rain_year_in) * 25.4), 4));
               
                root.davis_current_observation.temp_day_high_f = Convert.ToString(Math.Round((Convert.ToDouble(root.davis_current_observation.temp_day_high_f) - 32) * 5 / 9, 4));
                root.davis_current_observation.temp_day_low_f = Convert.ToString(Math.Round((Convert.ToDouble(root.davis_current_observation.temp_day_low_f) - 32) * 5 / 9, 4));

                return root;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<ResponseDavisDto> GetEstacionByFechaPagination(RequestDavisPaginadoDto dto)
        {
            try
            {
                ResponseDavisDto obj = new ResponseDavisDto();
                var existe = await _context.Estacion.AnyAsync(x => x.Id == dto.idPrimeraEstacion);
                var existeSecond = await _context.Estacion.AnyAsync(x => x.Id == dto.idSegundaEstacion);

                if (!existe && !existeSecond)
                {
                    obj.message = $"No existe ninguna estación seleccionada";
                    return obj;
                }
                if (!existe)
                {
                    obj.message = $"No se encontró la estación {dto.idPrimeraEstacion} seleccionada";
                    return obj;
                }
                if (!existeSecond)
                {
                    obj.message = $"No se encontró la estación {dto.idSegundaEstacion} seleccionada";
                    return obj;
                }

                string fechaInicio = dto.fechaInicio + " " + dto.horaInicio;
                string fechaFin = dto.fechaFin + " " + dto.horaFin;

                List<ResponseRootDavisDTO> lista = new List<ResponseRootDavisDTO>();

                decimal promedio = 0;
                decimal promedioSecond = 0;
                if (dto.idPrimeraEstacion != 0)
                {
                    var queryable = _context.DataDavis.Where(x => x.EstacionId == dto.idPrimeraEstacion && (x.fecha >= Convert.ToDateTime(fechaInicio) && x.fecha <= Convert.ToDateTime(fechaFin))).AsQueryable();
                    double cantidad = await queryable.CountAsync();
                    var dataFirst = await queryable.OrderByDescending(data => data.fecha).PaginarEstacion(dto).ToListAsync();

                    if (dataFirst.Count > 0)
                    {
                        List<DataDavisEntiti> PrimeraEstacion = new List<DataDavisEntiti>();
                        foreach (var davis in dataFirst)
                        {
                            if (davis.temp_c != "")
                            {
                                promedio += Convert.ToDecimal(davis.temp_c);
                            }
                            
                            PrimeraEstacion.Add(davis);
                        }
                        obj.promedioTempEstacion = Convert.ToString(Math.Round(promedio / dataFirst.Count(), 4));
                        obj.Estacion = PrimeraEstacion;
                        obj.totalEstacionOne = cantidad;

                    }
                }
                if (dto.idSegundaEstacion != 0)
                {
                    var queryable = _context.DataDavis.Where(x => x.EstacionId == dto.idSegundaEstacion && (x.fecha >= Convert.ToDateTime(fechaInicio) && x.fecha <= Convert.ToDateTime(fechaFin))).AsQueryable();
                    double cantidad = await queryable.CountAsync();
                    var dataSecond = await queryable.OrderByDescending(data => data.fecha).PaginarEstacion(dto).ToListAsync();
                    if (dataSecond.Count > 0)
                    {
                        List<DataDavisEntiti> SegundaEstacion = new List<DataDavisEntiti>();
                        foreach (var davis in dataSecond)
                        {
                            if (davis.temp_c != "")
                            {
                                promedioSecond += Convert.ToDecimal(davis.temp_c);
                            }
                            SegundaEstacion.Add(davis);
                        }
                        obj.promedioTempSegundaEstacion = Convert.ToString(Math.Round(promedioSecond / dataSecond.Count(), 4));
                        obj.SecondEstacion = SegundaEstacion;
                        obj.totalEstacionTwo = cantidad;

                    }
                }
                return obj;
            }
            catch (Exception)
            {

                throw;
            }
        }
        //public async Task<ResponseDavisDto> GetEstacionByFecha(int idPrimeraEstacion, int idSegundaEstacion, DateTime fecha)
        public async Task<ResponseDavisDto> GetEstacionByFecha(RequestDavisDto dto)
        {
            try
            {
                ResponseDavisDto obj = new ResponseDavisDto();
                var existe = await _context.Estacion.AnyAsync(x => x.Id == dto.idPrimeraEstacion);
                var existeSecond = await _context.Estacion.AnyAsync(x => x.Id == dto.idSegundaEstacion);

                if (!existe && !existeSecond)
                {
                    obj.message = $"No existe ninguna estación seleccionada";
                    return obj;
                }
                if (!existe)
                {
                    obj.message = $"No se encontró la estación {dto.idPrimeraEstacion} seleccionada";
                    return obj;
                }
                if (!existeSecond)
                {
                    obj.message = $"No se encontró la estación {dto.idSegundaEstacion} seleccionada";
                    return obj;
                }

                string fechaInicio = dto.fechaInicio + " " + dto.horaInicio;
                string fechaFin = dto.fechaFin + " " + dto.horaFin;

                List<ResponseRootDavisDTO> lista = new List<ResponseRootDavisDTO>();

                decimal promedio = 0;
                decimal promedioSecond = 0;
                if (dto.idPrimeraEstacion != 0)
                {
                    var dataFirst = await _context.DataDavis.Where(x => x.EstacionId == dto.idPrimeraEstacion && (x.fecha >= Convert.ToDateTime(fechaInicio) && x.fecha <= Convert.ToDateTime(fechaFin))).OrderByDescending(x=>x.fecha).ToListAsync();
                    if (dataFirst.Count > 0)
                    {
                        List<DataDavisEntiti> PrimeraEstacion = new List<DataDavisEntiti>();
                        foreach (var davis in dataFirst)
                        {
                            promedio += Convert.ToDecimal(davis.temp_c);
                            PrimeraEstacion.Add(davis);
                        }
                        obj.promedioTempEstacion = Convert.ToString(Math.Round(promedio / dataFirst.Count(), 4));
                        obj.Estacion = PrimeraEstacion;

                    }
                }
                if (dto.idSegundaEstacion != 0)
                {
                    var dataSecond = await _context.DataDavis.Where(x => x.EstacionId == dto.idSegundaEstacion && (x.fecha >= Convert.ToDateTime(fechaInicio) && x.fecha <= Convert.ToDateTime(fechaFin))).OrderByDescending(x => x.fecha).ToListAsync();
                    if (dataSecond.Count > 0)
                    {
                        List<DataDavisEntiti> SegundaEstacion = new List<DataDavisEntiti>();
                        foreach (var davis in dataSecond)
                        {
                            promedioSecond += Convert.ToDecimal(davis.temp_c);
                            SegundaEstacion.Add(davis);
                        }
                        obj.promedioTempSegundaEstacion = Convert.ToString(Math.Round(promedioSecond / dataSecond.Count(), 4));
                        obj.SecondEstacion = SegundaEstacion;

                    }
                }
                return obj;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<EstacionResponseDTO>> GetEstaciones()
        {

            try
            {
                var estaciones = await _context.Estacion.ToListAsync();

                var estacionResponse = mapper.Map<List<EstacionResponseDTO>>(estaciones);

                return estacionResponse;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> CrearEstacion(EstacionRequestDTO estacion)
        {
            try
            {
                var existeEstacion = await _context.Estacion.AnyAsync(p => p.Usuario == estacion.Usuario || p.Clave == estacion.Clave || p.Token == estacion.Token);

                if (existeEstacion)
                {
                    return existeEstacion;
                }


                var estaciones = mapper.Map<Estacion>(estacion);

                _context.Add(estaciones);
                await _context.SaveChangesAsync();
                return existeEstacion;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}

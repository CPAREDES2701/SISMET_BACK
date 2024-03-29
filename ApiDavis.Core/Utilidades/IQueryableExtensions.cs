﻿using ApiDavis.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Utilidades
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
        {
            return queryable
                .Skip((paginacionDTO.Pagina-1) * paginacionDTO.RecordsPorPagina)
                .Take(paginacionDTO.RecordsPorPagina);
        }
        public static IQueryable<T> PaginarEstacion<T>(this IQueryable<T> queryable, RequestDavisPaginadoDto paginacionDTO)
        {
            return queryable
                .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordsPorPagina)
                .Take(paginacionDTO.RecordsPorPagina);
        }
    }
}

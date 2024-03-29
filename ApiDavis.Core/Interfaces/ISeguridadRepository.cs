﻿using ApiDavis.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Core.Interfaces
{
    public interface ISeguridadRepository
    {
        Task<JwtResponse> Autenticar(UsuarioLoginDTO usuario);
    }
}

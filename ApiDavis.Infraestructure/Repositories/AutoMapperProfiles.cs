using ApiDavis.Core.DTOs;
using ApiDavis.Core.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDavis.Infraestructure.Repositories
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UsuarioRequestDTO, Usuario>();
            CreateMap<Usuario, UsuarioRequestDTO>();
        }
    }
}

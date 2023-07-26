using AutoMapper;
using BackEnd.Models;

namespace BackEnd.Mappers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Configurar el mapeo de Usuario a Cliente
            CreateMap<Usuario, Cliente>()
                .ForMember(dest => dest.Nombres, opt => opt.MapFrom(src => src.Nombres))
                .ForMember(dest => dest.Apellidos, opt => opt.MapFrom(src => src.Apellidos))
                .ForMember(dest => dest.Dni, opt => opt.MapFrom(src => src.Dni))
                .ForMember(dest => dest.Celular, opt => opt.MapFrom(src => src.Celular))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            // Configurar el mapeo de Cliente a Usuario
            CreateMap<Cliente, Usuario>()
                .ForMember(dest => dest.Nombres, opt => opt.MapFrom(src => src.Nombres))
                .ForMember(dest => dest.Apellidos, opt => opt.MapFrom(src => src.Apellidos))
                .ForMember(dest => dest.Dni, opt => opt.MapFrom(src => src.Dni))
                .ForMember(dest => dest.Celular, opt => opt.MapFrom(src => src.Celular))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}
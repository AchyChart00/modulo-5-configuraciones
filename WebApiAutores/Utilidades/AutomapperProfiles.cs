using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades;

public class AutomapperProfiles:Profile
{
    public AutomapperProfiles()
    {
        CreateMap<AutorCreacionDTO, Autor>();
    }
}
using AutoMapper;
using Domain;
using Domain.Models;

namespace Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Ingredient, Ingredient>();
        CreateMap<Recipe, Recipe>();
        CreateMap<Measurement, Measurement>();
        CreateMap<NutritionalInfo, NutritionalInfo>();
        CreateMap<MeasurementUnit, MeasurementUnit>();
    }
}
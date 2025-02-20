using Application.DayPlan;
using Application.DayPlan.DTOs;
using Application.DayPlanRecipe;
using Application.Ingredients;
using Application.Measurements;
using Application.Recipes;
using Application.WeekPlan;
using AutoMapper;
using Domain;
using Domain.Enums;
using Domain.Models;

namespace Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<NutritionalInfo, NutritionalInfo>();
        CreateMap<NutritionalInfo, NutritionalInfoDto>();
        CreateMap<NutritionalInfoDto, NutritionalInfo>();
        CreateMap<MeasurementUnit, MeasurementUnit>();
        
        CreateMap<Ingredient, IngredientDto>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.ToString()))
            .ForMember(d => d.MeasuredIn, 
                o => o.MapFrom(s => s.MeasurementUnit != null 
                    ? s.MeasurementUnit.MeasuredIn.ToString() 
                    : null))
            .ForMember(d => d.Nutrition, o => o.MapFrom(s => 
                s.Nutrition != null 
                    ? new NutritionalInfoDto 
                    {
                        IngredientId = s.Id,
                        Calories = s.Nutrition.Calories,
                        Carbs = s.Nutrition.Carbs,
                        Fat = s.Nutrition.Fat,
                        Protein = s.Nutrition.Protein
                    } 
                    : null
            ));

        CreateMap<IngredientDto, Ingredient>()
            .ForMember(d => d.Category, o => o.MapFrom(s => Enum.Parse<Categories.IngredientCategory>(s.Category)))
            .ForMember(d => d.MeasurementUnit, o => o.Ignore())
            .ForMember(d => d.Nutrition, o => o.MapFrom((src, dest) => 
                src.Nutrition != null 
                    ? new NutritionalInfo 
                    {
                        IngredientId = dest.Id != Guid.Empty ? dest.Id : Guid.NewGuid(),
                        Calories = src.Nutrition.Calories,
                        Carbs = src.Nutrition.Carbs,
                        Fat = src.Nutrition.Fat,
                        Protein = src.Nutrition.Protein
                    } 
                    : null
            ))
            .ForMember(d => d.AppUserId, o => o.MapFrom(s => s.AppUserId));

        CreateMap<Measurement, MeasurementDto>()
            .ForMember(d => d.RecipeName, o => o.MapFrom(s => s.Recipe.Name))
            .ForMember(d => d.IngredientName, o => o.MapFrom(s => s.Ingredient.Name))
            .ForMember(d => d.IngredientPricePerMeasurement, o => 
                o.MapFrom(s => s.Ingredient.MeasurementsPerPackage > 0 
                    ? (double)s.Ingredient.PricePerPackage / s.Ingredient.MeasurementsPerPackage 
                    : 0))
            .ForMember(d => d.IngredientCalories, o => o.MapFrom(s => s.Ingredient.Nutrition.Calories))
            .ForMember(d => d.IngredientCarbs, o => o.MapFrom(s => s.Ingredient.Nutrition.Carbs))
            .ForMember(d => d.IngredientFat, o => o.MapFrom(s => s.Ingredient.Nutrition.Fat))
            .ForMember(d => d.IngredientProtein, o => o.MapFrom(s => s.Ingredient.Nutrition.Protein));

        CreateMap<MeasurementDto, Measurement>()
            .ForMember(d => d.Recipe, o => o.Ignore())
            .ForMember(d => d.Ingredient, o => o.Ignore());

        CreateMap<Recipe, RecipeDto>()
            .ForMember(d => d.RecipeCategory, o => o.MapFrom(s => s.RecipeCategory.ToString()))
            .ForMember(d => d.Instructions, o => o.MapFrom(s => ConvertInstructionsJsonToList(s.InstructionsJson)))
            .ForMember(d => d.Nutrition, o => o.MapFrom((s, d) => {
                if (s.Measurements == null || !s.Measurements.Any())
                {
                    Console.WriteLine($"No measurements for recipe {s.Id}");
                    return null;
                }

                var nutritionDto = new RecipeNutritionDto
                {
                    RecipeId = s.Id,
                    TotalCalories = s.Measurements.Sum(m =>
                        Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Calories : 0)),
                    TotalCarbs = s.Measurements.Sum(m =>
                        Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Carbs : 0)),
                    TotalFat = s.Measurements.Sum(m =>
                        Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Fat : 0)),
                    TotalProtein = s.Measurements.Sum(m =>
                        Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Protein : 0)),
                    CaloriesPerServing = s.ServingsPerRecipe > 0
                        ? s.Measurements.Sum(m =>
                            Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Calories : 0)) / s.ServingsPerRecipe
                        : 0,
                    CarbsPerServing = s.ServingsPerRecipe > 0
                        ? s.Measurements.Sum(m =>
                            Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Carbs : 0)) / s.ServingsPerRecipe
                        : 0,
                    FatPerServing = s.ServingsPerRecipe > 0
                        ? s.Measurements.Sum(m =>
                            Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Fat : 0)) / s.ServingsPerRecipe
                        : 0,
                    ProteinPerServing = s.ServingsPerRecipe > 0
                        ? s.Measurements.Sum(m =>
                            Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Protein : 0)) / s.ServingsPerRecipe
                        : 0
                };

                Console.WriteLine($"Recipe {s.Id} Nutrition - Total Calories: {nutritionDto.TotalCalories}");

                return nutritionDto;
            }));

        CreateMap<RecipeDto, Recipe>()
            .ForMember(d => d.RecipeCategory, o => o.MapFrom(s => 
                Enum.Parse<Categories.RecipeCategory>(s.RecipeCategory)))
            .ForMember(d => d.InstructionsJson, o => o.MapFrom(s => 
                s.Instructions != null ? string.Join(";", s.Instructions) : ""))
            .ForMember(d => d.AppUserId, o => o.MapFrom(s => s.AppUserId))
            .ForMember(d => d.Measurements, o => o.MapFrom((src, dest) => 
                src.Measurements.Select(m => new Measurement
                {
                    Id = Guid.NewGuid(),
                    RecipeId = dest.Id,
                    IngredientId = m.IngredientId,
                    Amount = m.Amount
                }).ToList()
            ));
        
        CreateMap<Domain.Models.DayPlanRecipe, DayPlanRecipeDto>()
            .ForMember(d => d.RecipeName, o => o.MapFrom(s => s.Recipe.Name))
            .ForMember(d => d.RecipeId, o => o.MapFrom(s => s.RecipeId));

        CreateMap<DayPlanRecipeDto, Domain.Models.DayPlanRecipe>()
            .ForMember(d => d.Recipe, o => o.Ignore())
            .ForMember(d => d.DayPlan, o => o.Ignore());
        
        CreateMap<Domain.Models.DayPlan, DayPlanDto>()
            .ForMember(d => d.DayPlanRecipes, o => o.MapFrom(s => s.DayPlanRecipes))
            .ForMember(d => d.PriceForDay, o => o.Ignore())
            .ForMember(d => d.CaloriesPerDay, o => o.Ignore())
            .ForMember(d => d.CarbsPerDay, o => o.Ignore())
            .ForMember(d => d.FatPerDay, o => o.Ignore())
            .ForMember(d => d.ProteinPerDay, o => o.Ignore())
            .ForMember(d => d.DayPlanRecipes, o => o.NullSubstitute(new List<DayPlanRecipeDto>()));
        
        CreateMap<DayPlanDto, Domain.Models.DayPlan>()
            .ForMember(d => d.DayPlanRecipes, o => o.Ignore());
        
        CreateMap<Domain.Models.WeekPlan, WeekPlanDto>()
            .ForMember(d => d.DayPlans, o => o.MapFrom(s => s.DayPlans))
            .ForMember(d => d.PriceForWeek, o => o.Ignore())
            .ForMember(d => d.CaloriesPerWeek, o => o.Ignore())
            .ForMember(d => d.CarbsPerWeek, o => o.Ignore())
            .ForMember(d => d.FatPerWeek, o => o.Ignore())
            .ForMember(d => d.ProteinPerWeek, o => o.Ignore())
            .ForMember(d => d.DayPlans, o => o.NullSubstitute(new List<DayPlanDto>()));
       
        CreateMap<WeekPlanDto, Domain.Models.WeekPlan>()
            .ForMember(d => d.DayPlans, o => o.Ignore());
    }
    
    private static List<string> ConvertInstructionsJsonToList(string instructionsJson)
    {
        return string.IsNullOrEmpty(instructionsJson) ? new List<string>() : instructionsJson.Split(';').ToList();
    }
}


/*using Application.DayPlan;
using Application.DayPlan.DTOs;
using Application.DayPlanRecipe;
using Application.Ingredients;
using Application.Measurements;
using Application.Recipes;
using Application.WeekPlan;
using AutoMapper;
using Domain;
using Domain.Enums;
using Domain.Models;

namespace Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<NutritionalInfo, NutritionalInfo>();
        CreateMap<NutritionalInfo, NutritionalInfoDto>();
        CreateMap<NutritionalInfoDto, NutritionalInfo>();
        CreateMap<MeasurementUnit, MeasurementUnit>();
        
        CreateMap<Ingredient, IngredientDto>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.ToString()))
            .ForMember(d => d.MeasuredIn, 
                o => o.MapFrom(s => s.MeasurementUnit != null 
                    ? s.MeasurementUnit.MeasuredIn.ToString() 
                    : null))
            .ForMember(d => d.Nutrition, o => o.MapFrom(s => 
                s.Nutrition != null 
                    ? new NutritionalInfoDto 
                    {
                        IngredientId = s.Id,
                        Calories = s.Nutrition.Calories,
                        Carbs = s.Nutrition.Carbs,
                        Fat = s.Nutrition.Fat,
                        Protein = s.Nutrition.Protein
                    } 
                    : null
            ));

        CreateMap<IngredientDto, Ingredient>()
            .ForMember(d => d.Category, o => o.MapFrom(s => Enum.Parse<Categories.IngredientCategory>(s.Category)))
            .ForMember(d => d.MeasurementUnit, o => o.Ignore())
            .ForMember(d => d.Nutrition, o => o.MapFrom((src, dest) => 
                src.Nutrition != null 
                    ? new NutritionalInfo 
                    {
                        IngredientId = dest.Id != Guid.Empty ? dest.Id : Guid.NewGuid(),
                        Calories = src.Nutrition.Calories,
                        Carbs = src.Nutrition.Carbs,
                        Fat = src.Nutrition.Fat,
                        Protein = src.Nutrition.Protein
                    } 
                    : null
            ))
            .ForMember(d => d.AppUserId, o => o.MapFrom(s => s.AppUserId));
        // Model to Dto
        
        // Ingredient to IngredientDto
        /*
        CreateMap<Ingredient, IngredientDto>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.ToString())) // ✅ Convert Enum to String
            .ForMember(d => d.MeasuredIn,
                o => o.MapFrom(s => s.MeasurementUnit.MeasuredIn.ToString())) // ✅ Convert Enum to String
            .ForMember(d => d.Calories, o => o.MapFrom(s => s.Nutrition.Calories)) // ✅ Flatten NutritionalInfo
            .ForMember(d => d.Carbs, o => o.MapFrom(s => s.Nutrition.Carbs))
            .ForMember(d => d.Fat, o => o.MapFrom(s => s.Nutrition.Fat))
            .ForMember(d => d.Protein, o => o.MapFrom(s => s.Nutrition.Protein));
            #1#
        
        // Measurement to MeasurementDto
        CreateMap<Measurement, MeasurementDto>()
            .ForMember(d => d.RecipeName, o => o.MapFrom(s => s.Recipe.Name))
            .ForMember(d => d.IngredientName, o => o.MapFrom(s => s.Ingredient.Name))

            // Compute PricePerMeasurement during mapping
            .ForMember(d => d.IngredientPricePerMeasurement, o => 
                o.MapFrom(s => s.Ingredient.MeasurementsPerPackage > 0 
                    ? (double)s.Ingredient.PricePerPackage / s.Ingredient.MeasurementsPerPackage 
                    : 0))
    
            // Map flattened nutritional values
            .ForMember(d => d.IngredientCalories, o => o.MapFrom(s => s.Ingredient.Nutrition.Calories))
            .ForMember(d => d.IngredientCarbs, o => o.MapFrom(s => s.Ingredient.Nutrition.Carbs))
            .ForMember(d => d.IngredientFat, o => o.MapFrom(s => s.Ingredient.Nutrition.Fat))
            .ForMember(d => d.IngredientProtein, o => o.MapFrom(s => s.Ingredient.Nutrition.Protein));

        CreateMap<Recipe, RecipeDto>()
            .ForMember(d => d.RecipeCategory, o => o.MapFrom(s => s.RecipeCategory.ToString()))
            .ForMember(d => d.Instructions, o => o.MapFrom(s => ConvertInstructionsJsonToList(s.InstructionsJson)))
            .ForMember(d => d.Nutrition, o => o.MapFrom((s, d) => {
                if (s.Measurements == null || !s.Measurements.Any())
                {
                    Console.WriteLine($"No measurements for recipe {s.Id}");
                    return null;
                }

                var nutritionDto = new RecipeNutritionDto
                {
                    RecipeId = s.Id,
                    TotalCalories = s.Measurements.Sum(m =>
                        Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Calories : 0)),
                    TotalCarbs = s.Measurements.Sum(m =>
                        Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Carbs : 0)),
                    TotalFat = s.Measurements.Sum(m =>
                        Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Fat : 0)),
                    TotalProtein = s.Measurements.Sum(m =>
                        Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Protein : 0)),
                    CaloriesPerServing = s.ServingsPerRecipe > 0
                        ? s.Measurements.Sum(m =>
                            Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Calories : 0)) / s.ServingsPerRecipe
                        : 0,
                    CarbsPerServing = s.ServingsPerRecipe > 0
                        ? s.Measurements.Sum(m =>
                            Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Carbs : 0)) / s.ServingsPerRecipe
                        : 0,
                    FatPerServing = s.ServingsPerRecipe > 0
                        ? s.Measurements.Sum(m =>
                            Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Fat : 0)) / s.ServingsPerRecipe
                        : 0,
                    ProteinPerServing = s.ServingsPerRecipe > 0
                        ? s.Measurements.Sum(m =>
                            Convert.ToDouble(m.Amount) * (m.Ingredient.Nutrition != null ? m.Ingredient.Nutrition.Protein : 0)) / s.ServingsPerRecipe
                        : 0
                };

                 Console.WriteLine($"Recipe {s.Id} Nutrition - Total Calories: {nutritionDto.TotalCalories}");

                 return nutritionDto;
                 }
            ));
        CreateMap<RecipeDto, Recipe>()
            .ForMember(d => d.RecipeCategory, o => o.MapFrom(s => 
                Enum.Parse<Categories.RecipeCategory>(s.RecipeCategory)))
            .ForMember(d => d.InstructionsJson, o => o.MapFrom(s => 
                s.Instructions != null ? string.Join(";", s.Instructions) : ""))
            .ForMember(d => d.AppUserId, o => o.MapFrom(s => s.AppUserId))
            .ForMember(d => d.Measurements, o => o.MapFrom((src, dest) => 
                src.Measurements.Select(m => new Measurement
                {
                    Id = Guid.NewGuid(),
                    RecipeId = dest.Id,
                    IngredientId = m.IngredientId,
                    Amount = m.Amount
                }).ToList()
            ));
        
        /*CreateMap<RecipeDto, Recipe>()
            .ForMember(d => d.RecipeCategory, o => o.MapFrom(s => Enum.Parse<Categories.RecipeCategory>(s.RecipeCategory)))
            .ForMember(d => d.InstructionsJson, o => o.MapFrom(s => s.Instructions != null ? string.Join(";", s.Instructions) : ""))
            // Optional: Handle the Nutrition mapping if needed
            .ForMember(d => d.AppUserId, o => o.MapFrom(s => s.AppUserId));
        /*#1#
        CreateMap<Recipe, RecipeDto>()
            .ForMember(d => d.RecipeCategory, o => o.MapFrom(s => s.RecipeCategory.ToString())) // ✅ Convert Enum to String
            .ForMember(d => d.Instructions, o => o.MapFrom(s => ConvertInstructionsJsonToList(s.InstructionsJson))); // ✅ Use helper method
            

        /*
        CreateMap<RecipeDto, Recipe>()
            .ForMember(d => d.RecipeCategory, o => o.MapFrom(s => Enum.Parse<Categories.RecipeCategory>(s.RecipeCategory))) // ✅ Convert String to Enum
            .ForMember(d => d.InstructionsJson, o => o.MapFrom(s => s.Instructions != null ? string.Join(";", s.Instructions) : "")); // ✅ Join List to String
            #1#

        // Recipe to RecipeDto mapping

        // Dtos to Model mapping
        // IngredientDto to Ingredient
        /*
        CreateMap<IngredientDto, Ingredient>()
            .ForMember(d => d.Category, o => o.MapFrom(s => Enum.Parse<Categories.IngredientCategory>(s.Category)))
            .ForMember(d => d.MeasurementUnit, o => o.Ignore()) // ❌ DO NOT create a new MeasurementUnit
            .ForMember(d => d.Nutrition, o => o.MapFrom((s, d) => d.Nutrition ?? new NutritionalInfo
            {
                IngredientId = d.Id,  
                Calories = s.Calories,
                Carbs = s.Carbs,
                Fat = s.Fat,
                Protein = s.Protein
            }))
            .ForMember(d => d.AppUserId, o => o.MapFrom(s => s.AppUserId)); // ✅ If needed
            #1#




        
        // DayPlanRecipe Mappings
        CreateMap<Domain.Models.DayPlanRecipe, DayPlanRecipeDto>()
            .ForMember(d => d.RecipeName, o => o.MapFrom(s => s.Recipe.Name))
            .ForMember(d => d.RecipeId, o => o.MapFrom(s => s.RecipeId));

        CreateMap<DayPlanRecipeDto, Domain.Models.DayPlanRecipe>()
            .ForMember(d => d.Recipe, o => o.Ignore())
            .ForMember(d => d.DayPlan, o => o.Ignore());
        
        // DayPlan Mappings
        CreateMap<Domain.Models.DayPlan, DayPlanDto>()
            .ForMember(d => d.DayPlanRecipes, o => o.MapFrom(s => s.DayPlanRecipes)) // ✅ Map the list of recipes
            .ForMember(d => d.PriceForDay, o => o.Ignore()) // 🚫 Remove calculations from AutoMapper
            .ForMember(d => d.CaloriesPerDay, o => o.Ignore()) // 🚫 These will be set manually later
            .ForMember(d => d.CarbsPerDay, o => o.Ignore())
            .ForMember(d => d.FatPerDay, o => o.Ignore())
            .ForMember(d => d.ProteinPerDay, o => o.Ignore())
            .ForMember(d => d.DayPlanRecipes, o => o.NullSubstitute(new List<DayPlanRecipeDto>())); // ✅ Prevents null lists
        
        CreateMap<DayPlanDto, Domain.Models.DayPlan>()
            .ForMember(d => d.DayPlanRecipes, o => o.Ignore());
        
        // WeekPlan Mappings
        CreateMap<Domain.Models.WeekPlan, WeekPlanDto>()
            .ForMember(d => d.DayPlans, o => o.MapFrom(s => s.DayPlans)) // ✅ Maps list of DayPlans
            .ForMember(d => d.PriceForWeek, o => o.Ignore()) // 🚫 Remove calculations from AutoMapper
            .ForMember(d => d.CaloriesPerWeek, o => o.Ignore())
            .ForMember(d => d.CarbsPerWeek, o => o.Ignore())
            .ForMember(d => d.FatPerWeek, o => o.Ignore())
            .ForMember(d => d.ProteinPerWeek, o => o.Ignore())
            .ForMember(d => d.DayPlans, o => o.NullSubstitute(new List<DayPlanDto>())); // ✅ Prevents null lists
       
        CreateMap<WeekPlanDto, Domain.Models.WeekPlan>()
            .ForMember(d => d.DayPlans, o => o.Ignore());
        
    }
    
    
    
    
    
    private static List<string> ConvertInstructionsJsonToList(string instructionsJson)
    {
        return string.IsNullOrEmpty(instructionsJson) ? new List<string>() : instructionsJson.Split(';').ToList();
    }

}*/
using System.ComponentModel.DataAnnotations.Schema;
using Application.Measurements;
using Domain.Enums;
using Domain.Models;

namespace Application.Recipes;

public class RecipeDto
{

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string RecipeCategory { get; set; }
    public int ServingsPerRecipe { get; set; }

    /*// JSON conversion for EF to store instructions as a string in the database
    public string InstructionsJson
    {
        get => string.Join(";", Instructions);
        set => Instructions = value.Split(';').ToList();
    }*/
    public List<string> Instructions { get; set; }
    
    // List of related measurements
    public List<MeasurementDto> Measurements { get; set; } = new List<MeasurementDto>();

    //public AppUser AppUser { get; set; } // Navigation property
    public string AppUserId { get; set; } // Foreign key
    
    // ✅ Computed properties for pricing and nutrition
    public decimal TotalPrice { get; set; }
    public decimal PricePerServing { get; set; }
    public double CaloriesPerRecipe { get; set; }
    public double CarbsPerRecipe { get; set; }
    public double FatPerRecipe { get; set; }
    public double ProteinPerRecipe { get; set; }
    
    public RecipeNutritionDto Nutrition { get; set; }
    /*public decimal TotalPrice => Measurements.Sum(m => m.PricePerAmount);
    public decimal PricePerServing => ServingsPerRecipe > 0 ? TotalPrice / ServingsPerRecipe : 0;

    public double CaloriesPerRecipe => Measurements.Sum(m => m.CaloriesPerAmount);
    public double CarbsPerRecipe => Measurements.Sum(m => m.CarbsPerAmount);
    public double FatPerRecipe => Measurements.Sum(m => m.FatPerAmount);
    public double ProteinPerRecipe => Measurements.Sum(m => m.ProteinPerAmount);*/
}

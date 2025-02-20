using System.ComponentModel.DataAnnotations;
using Application.Profiles;
using Domain.Enums;
using Domain.Models;

namespace Application.Ingredients;

public class IngredientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal PricePerPackage { get; set; }
    public int MeasurementsPerPackage { get; set; }
    
    // Strings for category and measuredIn enums
    public string Category { get; set; }
    public string MeasuredIn { get; set; }
  
    // Nested NutritionalInfo
    public NutritionalInfoDto? Nutrition { get; set; }
    /*
    // Flattened NutritionalInfo
    public double Calories { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
    public double Protein { get; set; }
    */

    // AppUser properties simplified
    public string AppUserId { get; set; } // Exposing just the ID
    public string? AppUserDisplayName { get; set; } // Optional display name of the user
    
    // Price Per Measurement (computed dynamically)
    public decimal PricePerMeasurement => 
        MeasurementsPerPackage > 0 ? PricePerPackage / MeasurementsPerPackage : 0;
}
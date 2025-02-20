using Domain.Models;

namespace Application.Measurements;

public class MeasurementDto
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Amount { get; set; }
    
    // Expanded details
    public string RecipeName { get; set; }
    public string IngredientName { get; set; }
    public double IngredientPricePerMeasurement { get; set; }
    public double IngredientCalories { get; set; }
    public double IngredientCarbs { get; set; }
    public double IngredientFat { get; set; }
    public double IngredientProtein { get; set; }
    public decimal PricePerAmount { get; set; }
    public double CaloriesPerAmount { get; set; }
    public double CarbsPerAmount { get; set; }
    public double FatPerAmount { get; set; }
    public double ProteinPerAmount { get; set; }
}

/*using Domain.Models;

namespace Application.Measurements;

public class MeasurementDto
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Amount { get; set; }
    
    // ✅ Instead of full navigation properties, include key details
    public string RecipeName { get; set; }
    public string IngredientName { get; set; }
    
    // Flattened values from IngredientDto
    public double IngredientPricePerMeasurement { get; set; }
    public double IngredientCalories { get; set; }
    public double IngredientCarbs { get; set; }
    public double IngredientFat { get; set; }
    public double IngredientProtein { get; set; }
    
    // ✅ Computed properties based on Measurement Amount
    public decimal PricePerAmount => (decimal)IngredientPricePerMeasurement * Amount;
    public double CaloriesPerAmount => IngredientCalories * (double)Amount;
    public double CarbsPerAmount => IngredientCarbs * (double)Amount;
    public double FatPerAmount => IngredientFat * (double)Amount;
    public double ProteinPerAmount => IngredientProtein * (double)Amount;
}*/
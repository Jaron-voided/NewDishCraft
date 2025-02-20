namespace Application.Recipes;


public class RecipeNutritionDto
{
    public Guid RecipeId { get; set; }
    public double TotalCalories { get; set; }
    public double TotalCarbs { get; set; }
    public double TotalFat { get; set; }
    public double TotalProtein { get; set; }

    public double CaloriesPerServing { get; set; }
    public double CarbsPerServing { get; set; }
    public double FatPerServing { get; set; }
    public double ProteinPerServing { get; set; }
}
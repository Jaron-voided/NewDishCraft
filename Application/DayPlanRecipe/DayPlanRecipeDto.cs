namespace Application.DayPlanRecipe;
public class DayPlanRecipeDto
{
    public Guid Id { get; set; }
    public Guid DayPlanId { get; set; }
    public Guid RecipeId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public int Servings { get; set; }  // ✅ How many servings of this recipe in the DayPlan

    // ✅ Needed for calculations
    public decimal PricePerServing { get; set; }
    public decimal CaloriesPerRecipe { get; set; }
    public decimal CarbsPerRecipe { get; set; }
    public decimal ProteinPerRecipe { get; set; }
    public decimal FatPerRecipe { get; set; }
    public int ServingsPerRecipe { get; set; }
}
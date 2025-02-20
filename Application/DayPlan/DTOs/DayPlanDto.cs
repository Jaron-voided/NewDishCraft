
using Application.DayPlanRecipe;

namespace Application.DayPlan.DTOs;

public class DayPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AppUserId { get; set; } // Foreign key

    public List<DayPlanRecipeDto> DayPlanRecipes { get; set; } = new();
    
    // Computed Properties
    public decimal PriceForDay { get; set; }
    public decimal CaloriesPerDay { get; set; }
    public decimal CarbsPerDay { get; set; }
    public decimal FatPerDay { get; set; }
    public decimal ProteinPerDay { get; set; }
}
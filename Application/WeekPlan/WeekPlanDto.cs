using Application.DayPlan;
using Application.DayPlan.DTOs;
using Application.DayPlanRecipe;

namespace Application.WeekPlan;

public class WeekPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AppUserId { get; set; } // Foreign key

    public List<DayPlanDto> DayPlans { get; set; } = new();
    
    // Computed Properties
    public decimal PriceForWeek { get; set; }
    public decimal CaloriesPerWeek { get; set; }
    public decimal CarbsPerWeek { get; set; }
    public decimal FatPerWeek { get; set; }
    public decimal ProteinPerWeek { get; set; }
}
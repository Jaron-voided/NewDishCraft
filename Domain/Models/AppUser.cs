using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; }
    public string Bio { get; set; }
    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    
    public ICollection<DayPlan> DayPlans { get; set; } = new List<DayPlan>();
    public ICollection<WeekPlan> WeekPlans { get; set; } = new List<WeekPlan>();
}
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Models;

public class DayPlan
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    //[JsonIgnore]
    public ICollection<DayPlanRecipe> DayPlanRecipes { get; set; } = new List<DayPlanRecipe>();
    
    //[JsonIgnore]
    public virtual AppUser? AppUser { get; set; } // Navigation property
    //[JsonIgnore]
    public string AppUserId { get; set; } = string.Empty; // Foreign key
    
    public Guid? WeekPlanId { get; set; }
    [JsonIgnore]
    public WeekPlan? WeekPlan { get; set; }

}
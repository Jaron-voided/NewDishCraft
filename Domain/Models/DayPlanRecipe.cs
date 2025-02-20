using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Models;

public class DayPlanRecipe
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid DayPlanId { get; set; } 
    [JsonIgnore]
    public DayPlan? DayPlan { get; set; } // ✅ Navigation property to DayPlan

    [Required]
    public Guid RecipeId { get; set; }
    [JsonIgnore]
    public Recipe? Recipe { get; set; } // ✅ Navigation property to Recipe

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Servings must be at least 1.")]
    public int Servings { get; set; }
}

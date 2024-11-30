using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public class NutritionalInfo
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("Ingredient")]
    public Guid IngredientId { get; set; }
    
    [Required]
    public double Calories { get; set; }
    [Required]
    public double Carbs { get; set; }
    [Required]
    public double Fat { get; set; }
    [Required]
    public double Protein { get; set; }

    public Ingredient Ingredient { get; set; } // Navigation property
}
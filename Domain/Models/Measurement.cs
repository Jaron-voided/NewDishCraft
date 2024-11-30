using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public class Measurement
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey("Recipe")]
    public Guid RecipeId { get; set; }

    [Required]
    [ForeignKey("Ingredient")]
    public Guid IngredientId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    // Navigation Properties
    public Recipe Recipe { get; set; }
    public Ingredient Ingredient { get; set; }
}
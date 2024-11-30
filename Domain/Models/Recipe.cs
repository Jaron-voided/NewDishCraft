using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Models;

public class Recipe
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    [Required]
    public Categories.RecipeCategory RecipeCategory { get; set; }
 
    [Required]
    public int ServingsPerRecipe { get; set; }

    // JSON conversion for EF to store instructions as a string in the database
    [Required]
    public string InstructionsJson
    {
        get => string.Join(";", Instructions);
        set => Instructions = value.Split(';').ToList();
    }

    [NotMapped] // EF ignores this for database mapping
    public List<string> Instructions { get; set; }

    // Navigation Property
    public ICollection<Measurement> Measurements { get; set; }
}
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Models;

public class Ingredient
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [Required]
    public MeasurementUnit MeasurementUnit { get; set; }
    
    [Required]
    public Categories.IngredientCategory Category { get; set; }
    
    [Required]
    public decimal PricePerPackage { get; set; }
    [Required]
    public int MeasurementsPerPackage { get; set; }
    
    public NutritionalInfo? Nutrition { get; set; }
}
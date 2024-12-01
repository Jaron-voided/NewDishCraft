using System.ComponentModel.DataAnnotations;
using Domain.Enums;

#nullable enable

namespace Domain.Models;

public class Ingredient
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [Required]
    public Categories.IngredientCategory Category { get; set; }
    
    [Required]
    public decimal PricePerPackage { get; set; }
    [Required]
    public int MeasurementsPerPackage { get; set; }
    
    public NutritionalInfo? Nutrition { get; set; }
    public MeasurementUnit? MeasurementUnit { get; set; }
}
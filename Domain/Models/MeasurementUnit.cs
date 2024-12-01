using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Models;

public class MeasurementUnit
{
    [Key, ForeignKey("Ingredient")]
    public Guid IngredientId { get; set; } // Primary Key and Foreign Key

    public MeasuredIn MeasuredIn { get; set; }

    public WeightUnit? WeightUnit { get; set; }

    public VolumeUnit? VolumeUnit { get; set; }

    public Ingredient Ingredient { get; set; } // Navigation property
}

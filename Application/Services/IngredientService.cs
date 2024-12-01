using Application.Utilities;
using Domain.Enums;
using Domain.Models;

namespace Application.Services;

public class IngredientService
{
    private readonly Conversions _conversions;

    public IngredientService(Conversions conversions)
    {
        _conversions = conversions;
    }

    public decimal ConvertToBaseUnit(Ingredient ingredient)
    {
        if (ingredient.MeasurementUnit == null)
            throw new InvalidOperationException("Ingredient must have a MeasurementUnit.");

        return ingredient.MeasurementUnit.MeasuredIn switch
        {
            MeasuredIn.Weight => ingredient.MeasurementUnit.WeightUnit switch
            {
                WeightUnit.Pounds => _conversions.ConvertPoundsToGrams(ingredient.MeasurementsPerPackage),
                WeightUnit.Ounces => _conversions.ConvertOuncesToGrams(ingredient.MeasurementsPerPackage),
                WeightUnit.Kilograms => _conversions.ConvertKilosToGrams(ingredient.MeasurementsPerPackage),
                _ => throw new InvalidOperationException("Unsupported weight unit.")
            },
            MeasuredIn.Volume => ingredient.MeasurementUnit.VolumeUnit switch
            {
                VolumeUnit.FluidOunces => _conversions.ConvertFluidOuncesToMilliliters(ingredient.MeasurementsPerPackage),
                VolumeUnit.Cups => _conversions.ConvertCupsToMilliliters(ingredient.MeasurementsPerPackage),
                VolumeUnit.Liters => _conversions.ConvertLitersToMilliliters(ingredient.MeasurementsPerPackage),
                VolumeUnit.Gallons => _conversions.ConvertGallonsToMilliliters(ingredient.MeasurementsPerPackage),
                _ => throw new InvalidOperationException("Unsupported volume unit.")
            },
            MeasuredIn.Each => ingredient.MeasurementsPerPackage,
            _ => throw new InvalidOperationException("Unsupported measurement type.")
        };
    }
}
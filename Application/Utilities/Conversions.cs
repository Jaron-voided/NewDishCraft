namespace Application.Utilities;

public class Conversions
{
    private const decimal GramsPerPound = 454;
    private const decimal GramsPerKilo = 1000;
    private const decimal GramsPerOunce = 28;

    private const decimal MillilitersPerFlOz = 29.57m;
    private const decimal MillilitersPerLiter = 1000;
    private const decimal MillilitersPerCup = 236.5m;
    private const decimal MillilitersPerGallon = 3785.41m;

    // Functions for converting to grams
    public decimal ConvertPoundsToGrams(decimal pounds) => pounds * GramsPerPound;

    public decimal ConvertKilosToGrams(decimal kilos) => kilos * GramsPerKilo;

    public decimal ConvertOuncesToGrams(decimal ounces) => ounces * GramsPerOunce;

    // Functions for converting to milliliters
    public decimal ConvertFluidOuncesToMilliliters(decimal fluidOunces) => fluidOunces * MillilitersPerFlOz;

    public decimal ConvertLitersToMilliliters(decimal liters) => liters * MillilitersPerLiter;

    public decimal ConvertCupsToMilliliters(decimal cups) => cups * MillilitersPerCup;

    public decimal ConvertGallonsToMilliliters(decimal gallons) => gallons * MillilitersPerGallon;
}
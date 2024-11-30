namespace Domain.Enums;

public class MeasurementUnit
{
    public MeasuredIn MeasuredIn { get; set; }
    public WeightUnit? WeightUnit { get; set; }
    public VolumeUnit? VolumeUnit { get; set; }
}

public enum MeasuredIn
{
    Weight = 0,
    Volume = 1,
    Each = 2
}

public enum WeightUnit
{
    Grams = 0,
    Kilograms = 1,
    Ounces = 2,
    Pounds = 3
}

public enum VolumeUnit
{
    Millimeters = 0,
    Liters = 1,
    Cups = 2,
    FluidOunces = 3,
    Gallons = 4
}
import { VolumeUnit } from "../../models/ingredient.ts";

export const volumeOptions = [
    { key: VolumeUnit.Milliliters, text: "Milliliters", value: VolumeUnit.Milliliters },
    { key: VolumeUnit.Liters, text: "Liters", value: VolumeUnit.Liters },
    { key: VolumeUnit.Cups, text: "Cups", value: VolumeUnit.Cups },
    { key: VolumeUnit.FluidOunces, text: "Fluid Ounces", value: VolumeUnit.FluidOunces },
    { key: VolumeUnit.Gallons, text: "Gallons", value: VolumeUnit.Gallons },
];

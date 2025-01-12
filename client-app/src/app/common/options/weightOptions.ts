import { WeightUnit } from "../../models/ingredient.ts";

export const weightOptions = [
    { key: WeightUnit.Grams, text: "Grams", value: WeightUnit.Grams },
    { key: WeightUnit.Kilograms, text: "Kilograms", value: WeightUnit.Kilograms },
    { key: WeightUnit.Ounces, text: "Ounces", value: WeightUnit.Ounces },
    { key: WeightUnit.Pounds, text: "Pounds", value: WeightUnit.Pounds },
];

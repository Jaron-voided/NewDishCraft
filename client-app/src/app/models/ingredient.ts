// Enums from your backend
export enum MeasuredIn {
    Weight = "Weight",
    Volume = "Volume",
    Each = "Each"
}

export enum WeightUnit {
    Grams = "Grams",
    Kilograms = "Kilograms",
    Ounces = "Ounces",
    Pounds = "Pounds"
}

export enum VolumeUnit {
    Milliliters = "Milliliters",
    Liters = "Liters",
    Cups = "Cups",
    FluidOunces = "FluidOunces",
    Gallons = "Gallons"
}

export enum IngredientCategory {
    Spice = "Spice",
    Meat = "Meat",
    Vegetable = "Vegetable",
    Fruit = "Fruit",
    Dairy = "Dairy",
    Grain = "Grain",
    Liquid = "Liquid",
    Baking = "Baking"
}


// Interfaces
export interface Ingredient {
    id: string
    name: string
    category: IngredientCategory
    pricePerPackage: number
    measurementsPerPackage: number
    nutrition?: Nutrition // Optional because it might not be set
    measurementUnit?: MeasurementUnit // Optional because it might not be set
}

export interface Nutrition {
    ingredientId: string
    calories: number
    carbs: number
    fat: number
    protein: number
    ingredient?: Ingredient | null // Optional and nullable
}

export interface MeasurementUnit {
    ingredientId: string
    measuredIn: MeasuredIn
    weightUnit?: WeightUnit | null // Optional and nullable
    volumeUnit?: VolumeUnit | null // Optional and nullable
    ingredient?: Ingredient | null // Optional and nullable
}

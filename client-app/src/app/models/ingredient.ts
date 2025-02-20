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
// Ingredient Model
export interface Ingredient {
    id: string;
    name: string;
    category: string;
    pricePerPackage: number;
    measurementsPerPackage: number;
    measurementUnit: MeasurementUnit;
    measuredIn?: string | null;
    nutrition?: NutritionalInfo | null;
    pricePerMeasurement?: number;
    appUserId?: string;
    appUserDisplayName?: string | null;
}
/*
export interface Ingredient {
    id: string;
    name: string;
    category: string; // Now a string instead of an enum
    //measuredIn: string; // Now a string instead of an enum
    pricePerPackage: number;
    measurementsPerPackage: number;

    measurementUnit: {
        ingredientId: string;
        measuredIn: MeasuredIn;
        weightUnit?: WeightUnit;
        volumeUnit?: VolumeUnit;
    };

    nutrition: {
        ingredientId: string;
        calories: number;
        protein: number;
        carbs: number;
        fat: number;
    };

    pricePerMeasurement?: number; // New computed property from the DTO
/!*

    // Flattened nutritional values
    calories: number;
    carbs: number;
    fat: number;
    protein: number;
*!/

    // User ownership properties
    appUserId?: string;
    appUserDisplayName?: string | null;
}
*/

export interface NutritionalInfo {
    ingredientId: string;
    calories: number;
    carbs: number;
    fat: number;
    protein: number;
}
/*export interface Nutrition {
    ingredientId: string
    calories: number
    carbs: number
    fat: number
    protein: number
    ingredient?: Ingredient | null // Optional and nullable
}*/

export interface MeasurementUnit {
    ingredientId: string
    measuredIn: MeasuredIn
    weightUnit?: WeightUnit | null // Optional and nullable
    volumeUnit?: VolumeUnit | null // Optional and nullable
    ingredient?: Ingredient | null // Optional and nullable
}

// Enums
// import {Ingredient} from "./ingredient.ts";

export enum RecipeCategory {
    Soup = "Soup",
    Appetizer = "Appetizer",
    Breakfast = "Breakfast",
    Lunch = "Lunch",
    Dinner = "Dinner",
    Dessert = "Dessert",
    Sauce = "Sauce"
}

export interface RecipeNutrition {
    recipeId: string;
    totalCalories: number;
    totalCarbs: number;
    totalFat: number;
    totalProtein: number;
    caloriesPerServing: number;
    carbsPerServing: number;
    fatPerServing: number;
    proteinPerServing: number;
}

// Interfaces
export interface Recipe {
    id: string;
    name: string;
    recipeCategory: string; // Now a string instead of an enum
    servingsPerRecipe: number;
    instructions: string[]; // Use array instead of JSON string
    appUserId: string; // Ensure it's present
    totalPrice: number;
    pricePerServing: number;
    caloriesPerRecipe: number;
    carbsPerRecipe: number;
    fatPerRecipe: number;
    proteinPerRecipe: number;

    measurements: Measurement[];
    nutrition?: RecipeNutrition;
/*    nutrition?: RecipeNutrition | null;

    measurements: {
        ingredientId: string;
        amount: number;
    }[];*/
}



export interface Measurement {
    id: string;
    recipeId: string;
    ingredientId: string;
    amount: number;
    recipeName: string;
    ingredientName: string;
    ingredientPricePerMeasurement: number;
    ingredientCalories: number;
    ingredientCarbs: number;
    ingredientFat: number;
    ingredientProtein: number;
    pricePerAmount: number;
    caloriesPerAmount: number;
    carbsPerAmount: number;
    fatPerAmount: number;
    proteinPerAmount: number;
}



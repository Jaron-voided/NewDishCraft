// Enums
import {Ingredient} from "./ingredient.ts";

export enum RecipeCategory {
    Soup = "Soup",
    Appetizer = "Appetizer",
    Breakfast = "Breakfast",
    Lunch = "Lunch",
    Dinner = "Dinner",
    Dessert = "Dessert",
    Sauce = "Sauce"
}


// Interfaces
export interface Recipe {
    id: string;
    name: string;
    recipeCategory: RecipeCategory; // Enum for recipe categories
    servingsPerRecipe: number;
    instructionsJson: string; // Backend-stored string for instructions
    instructions: string[]; // Array representation of instructions
    measurements: Measurement[]; // Array of measurements associated with the recipe
}

export interface Measurement {
    id: string;
    recipeId: string;
    ingredientId: string;
    amount: number;
    recipe?: Recipe | null; // Optional and nullable to avoid circular dependency issues
    ingredient?: Ingredient | null; // Optional and nullable, referencing your Ingredient interface
}

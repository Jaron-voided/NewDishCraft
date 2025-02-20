import { Recipe, Measurement } from '../models/recipe';
import IngredientStore from "../stores/ingredientStore.ts";

export function expandMeasurements(recipe: Recipe, ingredientStore: IngredientStore): Measurement[] {
    if (!recipe || !recipe.measurements) return [];

    return recipe.measurements.map(m => {
        const ingredient = ingredientStore.getIngredient(m.ingredientId);
        if (!ingredient) {
            throw new Error(`Ingredient with id ${m.ingredientId} not found`);
        }

        const pricePerMeasurement = ingredient.pricePerMeasurement ?? 0;

        return {
            id: `${recipe.id}-${m.ingredientId}`,
            recipeId: recipe.id,
            ingredientId: m.ingredientId,
            amount: m.amount,
            recipeName: recipe.name,
            ingredientName: ingredient.name,
            ingredientPricePerMeasurement: pricePerMeasurement,
            ingredientCalories: ingredient.nutrition.calories,
            ingredientCarbs: ingredient.nutrition.carbs,
            ingredientFat: ingredient.nutrition.fat,
            ingredientProtein: ingredient.nutrition.protein,
            pricePerAmount: m.amount * pricePerMeasurement,
            caloriesPerAmount: m.amount * ingredient.nutrition.calories,
            carbsPerAmount: m.amount * ingredient.nutrition.carbs,
            fatPerAmount: m.amount * ingredient.nutrition.fat,
            proteinPerAmount: m.amount * ingredient.nutrition.protein
        };
    });
}
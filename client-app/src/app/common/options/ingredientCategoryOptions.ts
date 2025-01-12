import { IngredientCategory } from "../../models/ingredient.ts";

export const ingredientCategoryOptions = [
    { key: IngredientCategory.Spice, text: "Spice", value: IngredientCategory.Spice },
    { key: IngredientCategory.Meat, text: "Meat", value: IngredientCategory.Meat },
    { key: IngredientCategory.Vegetable, text: "Vegetable", value: IngredientCategory.Vegetable },
    { key: IngredientCategory.Fruit, text: "Fruit", value: IngredientCategory.Fruit },
    { key: IngredientCategory.Dairy, text: "Dairy", value: IngredientCategory.Dairy },
    { key: IngredientCategory.Grain, text: "Grain", value: IngredientCategory.Grain },
    { key: IngredientCategory.Liquid, text: "Liquid", value: IngredientCategory.Liquid },
    { key: IngredientCategory.Baking, text: "Baking", value: IngredientCategory.Baking },
];

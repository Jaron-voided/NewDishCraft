import { RecipeCategory } from "../../models/recipe.ts";

export const recipeCategoryOptions = [
    { key: RecipeCategory.Soup, text: "Soup", value: RecipeCategory.Soup },
    { key: RecipeCategory.Appetizer, text: "Appetizer", value: RecipeCategory.Appetizer },
    { key: RecipeCategory.Breakfast, text: "Breakfast", value: RecipeCategory.Breakfast },
    { key: RecipeCategory.Lunch, text: "Lunch", value: RecipeCategory.Lunch },
    { key: RecipeCategory.Dinner, text: "Dinner", value: RecipeCategory.Dinner },
    { key: RecipeCategory.Dessert, text: "Dessert", value: RecipeCategory.Dessert },
    { key: RecipeCategory.Sauce, text: "Sauce", value: RecipeCategory.Sauce },
];

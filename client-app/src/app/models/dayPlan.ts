export interface DayPlan {
    id: string;
    name: string;
    appUserId?: string;

    dayPlanRecipes: DayPlanRecipe[];

    priceForDay: number;
    caloriesPerDay: number;
    carbsPerDay: number;
    fatPerDay: number;
    proteinPerDay: number;
}


export interface DayPlanRecipe {
    id: string;
    dayPlanId: string;
    recipeId: string;
    recipeName: string;
    servings: number;

    pricePerServing: number;
    caloriesPerRecipe: number;
    carbsPerRecipe: number;
    proteinPerRecipe: number;
    fatPerRecipe: number;
    servingsPerRecipe: number;
}

// In models/dayPlan.ts
export interface DayPlanDto {
    id: string;
    name: string;
    appUserId: string;
    dayPlanRecipes: DayPlanRecipeDto[];

    priceForDay: number;
    caloriesPerDay: number;
    carbsPerDay: number;
    fatPerDay: number;
    proteinPerDay: number;
}

export interface DayPlanRecipeDto {
    id: string;
    dayPlanId: string;
    recipeId: string;
    recipeName: string;
    servings: number;

    pricePerServing: number;
    caloriesPerRecipe: number;
    carbsPerRecipe: number;
    proteinPerRecipe: number;
    fatPerRecipe: number;
    servingsPerRecipe: number;
}

export interface DayPlanCalculationsDto {
    totalPrice: number;
    totalCalories: number;
    totalCarbs: number;
    totalFat: number;
    totalProtein: number;
}

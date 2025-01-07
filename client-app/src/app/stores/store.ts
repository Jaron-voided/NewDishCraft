import IngredientStore from "./ingredientStore.ts";
import {createContext, useContext} from "react";
import RecipeStore from "./recipeStore.ts";
import CommonStore from "./commonStore.ts";

interface Store {
    ingredientStore: IngredientStore
    recipeStore: RecipeStore
    commonStore: CommonStore;
}

export const store: Store = {
    ingredientStore: new IngredientStore(),
    recipeStore: new RecipeStore(),
    commonStore: new CommonStore()
}

export const StoreContext = createContext(store);

export function useStore() {
    return useContext(StoreContext);
}
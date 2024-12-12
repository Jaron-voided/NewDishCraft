import IngredientStore from "./ingredientStore.ts";
import {createContext, useContext} from "react";

interface Store {
    ingredientStore: IngredientStore
}

export const store: Store = {
    ingredientStore: new IngredientStore()
}

export const StoreContext = createContext(store);

export function useStore() {
    return useContext(StoreContext);
}
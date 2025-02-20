import IngredientStore from "./ingredientStore.ts";
import {createContext, useContext} from "react";
import RecipeStore from "./recipeStore.ts";
import CommonStore from "./commonStore.ts";
import UserStore from "./userStore.ts";
import ModalStore from "./modalStore.ts";
import DayPlanStore from "./dayPlanStore.ts";

interface Store {
    ingredientStore: IngredientStore;
    recipeStore: RecipeStore;
    dayPlanStore: DayPlanStore;
    commonStore: CommonStore;
    userStore: UserStore;
    modalStore: ModalStore;
}

export const store: Store = {
    ingredientStore: new IngredientStore(),
    recipeStore: new RecipeStore(),
    dayPlanStore: new DayPlanStore(),
    commonStore: new CommonStore(),
    userStore: new UserStore(),
    modalStore: new ModalStore()

}

export const StoreContext = createContext(store);

export function useStore() {
    return useContext(StoreContext);
}
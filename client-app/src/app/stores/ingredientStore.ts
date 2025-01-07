import {makeAutoObservable, runInAction} from "mobx";
import {Ingredient, MeasuredIn} from "../models/ingredient.ts";
import agent from "../api/agent.ts";
import {v4 as uuid} from 'uuid';

export default class IngredientStore {

    ingredientRegistry = new Map<string, Ingredient>();
    selectedIngredient: Ingredient | undefined = undefined;
    editMode = false;
    loading = false;
    loadingInitial = false;


    constructor() {
        makeAutoObservable(this)
    }

    get ingredientsByPrice() {
        return Array.from(this.ingredientRegistry.values()).sort((a, b) => a.pricePerPackage - b.pricePerPackage);
    }

    get ingredientsByCategory() {
        return Array.from(this.ingredientRegistry.values()).sort((a, b) =>
            Number(a.category) - Number(b.category)
        );
    }

    get groupedIngredients() {
        return Object.entries(
            this.ingredientsByCategory.reduce((ingredients, ingredient) => {
                const category = ingredient.category;
                if (!ingredients[category]) {
                    ingredients[category] = [];
                }
                ingredients[category].push(ingredient);
                return ingredients;
            }, {} as Record<string, Ingredient[]>)
        );
    }


    loadIngredients = async () => {
        this.setLoadingInitial(true);
        try {
            const ingredients = await agent.Ingredients.list();
            ingredients.forEach(ingredient => {
             /*   this.ingredientRegistry.set(ingredient.id, ingredient);*/
                this.setIngredient(ingredient);
            })

            this.setLoadingInitial(false);

        } catch (error) {
            console.log(error);
            runInAction(() => this.setLoadingInitial(false));
        }
    };

    loadIngredient = async (id: string) => {
        let ingredient = this.getIngredient(id);
        if (ingredient) {
            this.selectedIngredient = ingredient;
            return ingredient;
        }
        else {
            this.setLoadingInitial(true);
            try {
                ingredient = await agent.Ingredients.details(id);
                this.setIngredient(ingredient);
                runInAction(() => this.selectedIngredient = ingredient);
                this.setLoadingInitial(false);
                return ingredient;
            } catch (error) {
                console.log(error);
                this.setLoadingInitial(false);
            }
        }
    }

    private setIngredient = (ingredient: Ingredient) => {
        this.ingredientRegistry.set(ingredient.id, ingredient);
    }

    private getIngredient = (id: string) => {
        return this.ingredientRegistry.get(id);
    }


    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }


    createIngredient = async (ingredient: Ingredient) => {
        this.loading = true;

        try {
            ingredient.id = uuid();
            // Ensure nutrition is initialized
            if (!ingredient.nutrition) {
                ingredient.nutrition = {
                    ingredientId: ingredient.id,
                    calories: 0,
                    protein: 0,
                    carbs: 0,
                    fat: 0,
                };
            } else {
                ingredient.nutrition.ingredientId = ingredient.id;
            }

            // Ensure measurementUnit is initialized
            if (!ingredient.measurementUnit) {
                ingredient.measurementUnit = {
                    ingredientId: ingredient.id,
                    measuredIn: MeasuredIn.Weight, // Default to Weight
                    weightUnit: undefined,
                    volumeUnit: undefined,
                };
            } else {
                ingredient.measurementUnit.ingredientId = ingredient.id;
            }

            await agent.Ingredients.create(ingredient);
            runInAction(() => {
                this.ingredientRegistry.set(ingredient.id, ingredient);
                this.selectedIngredient = ingredient;
                this.editMode = false;
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }

    updateIngredient = async (ingredient: Ingredient) => {
        this.loading = true;
        try {
            await agent.Ingredients.update(ingredient);
            runInAction(() => {
                this.ingredientRegistry.set(ingredient.id, ingredient);
                this.selectedIngredient = ingredient;
                this.editMode = false;
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }

    deleteIngredient = async (id: string) => {
        this.loading = true;
        try {
            await agent.Ingredients.delete(id);
            runInAction(() => {
                this.ingredientRegistry.delete(id);
               /* if (this.selectedIngredient?.id === id) this.cancelSelectedIngredient();*/
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }
}
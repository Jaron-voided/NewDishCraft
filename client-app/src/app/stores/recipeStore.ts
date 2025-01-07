import {makeAutoObservable, runInAction} from "mobx";
import {Measurement, Recipe} from "../models/recipe.ts";
import agent from "../api/agent.ts";
import {v4 as uuid} from 'uuid';


export default class RecipeStore {

    recipeRegistry = new Map<string, Recipe>();
    selectedRecipe: Recipe | undefined = undefined;
    editMode = false;
    loading = false;
    loadingInitial = false;


    constructor() {
        makeAutoObservable(this)
    }

/*    get recipesByPrice() {
        return Array.from(this.recipeRegistry.values()).sort((a, b) => a.pricePerPackage - b.pricePerPackage);
    }*/

    get recipesByCategory() {
        return Array.from(this.recipeRegistry.values()).sort((a, b) =>
            Number(a.recipeCategory) - Number(b.recipeCategory)
        );
    }

    get groupedRecipes() {
        return Object.entries(
            this.recipesByCategory.reduce((recipes, recipe) => {
                const category = recipe.recipeCategory;
                if (!recipes[category]) {
                    recipes[category] = [];
                }
                recipes[category].push(recipe);
                return recipes;
            }, {} as Record<string, Recipe[]>)
        );
    }


    loadRecipes = async () => {
        this.setLoadingInitial(true);
        try {
            const recipes = await agent.Recipes.list();
            recipes.forEach(recipe => {
                /*   this.recipeRegistry.set(recipe.id, recipe);*/
                this.setRecipe(recipe);
            })

            this.setLoadingInitial(false);

        } catch (error) {
            console.log(error);
            runInAction(() => this.setLoadingInitial(false));
        }
    };

    loadRecipe = async (id: string) => {
        let recipe = this.getRecipe(id);
        if (recipe) {
            this.selectedRecipe = recipe;
            return recipe;
        }
        else {
            this.setLoadingInitial(true);
            try {
                recipe = await agent.Recipes.details(id);
                this.setRecipe(recipe);
                runInAction(() => this.selectedRecipe = recipe);
                this.setLoadingInitial(false);
                return recipe;
            } catch (error) {
                console.log(error);
                this.setLoadingInitial(false);
            }
        }
    }

    private setRecipe = (recipe: Recipe) => {
        this.recipeRegistry.set(recipe.id, recipe);
    }

    private getRecipe = (id: string) => {
        return this.recipeRegistry.get(id);
    }


    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    createMeasurement = async (recipeId: string, ingredientId: string, amount: number) => {
        try {
            const measurement = {
                id: uuid(),
                recipeId: recipeId,
                ingredientId: ingredientId,
                amount: amount
            };

            await agent.Measurements.create(measurement);

            runInAction(() => {
                const recipe = this.recipeRegistry.get(recipeId);
                if (recipe) {
                    recipe.measurements.push(measurement);
                    this.recipeRegistry.set(recipe.id, recipe);
                }
            });
        } catch (error) {
            console.log(error);
        }
    }

    createMeasurements = async (recipeId: string,
                                measurements: {ingredientId: string; amount: number}[]) => {
        try {
            for (const measurement of measurements) {
                await this.createMeasurement(recipeId, measurement.ingredientId, measurement.amount);
            }

            runInAction(() => {
                this.setLoadingInitial(false);
            });
        } catch (error){
            console.log(error);
            runInAction(() => this.setLoadingInitial(false));
        }
    }

    createRecipe = async (recipe: Recipe,
                          measurements?: {ingredientId: string; amount: number }[]) => {
        this.loading = true;

        try {
            recipe.id = uuid();
            await agent.Recipes.create(recipe);

            runInAction(() => {
                this.recipeRegistry.set(recipe.id, recipe);
                this.selectedRecipe = recipe;
            });

            if (measurements && measurements.length > 0) {
                await this.createMeasurements(recipe.id, measurements)
            }

            runInAction(() => {
                this.editMode = false;
                this.setLoadingInitial(false);
            });
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            });
        }
    };

    updateMeasurement = async (measurement: Measurement) => {
        try {
            await agent.Measurements.update(measurement); // API call to update measurement
            runInAction(() => {
                if (this.selectedRecipe) {
                    const index = this.selectedRecipe.measurements.findIndex(m => m.id === measurement.id);
                    if (index !== -1) {
                        this.selectedRecipe.measurements[index] = measurement; // Update existing measurement
                    }
                }
            });
        } catch (error) {
            console.log(error);
        }
    };


    updateRecipe = async (recipe: Recipe) => {
        this.loading = true;

        try {
            // Step 1: Update the recipe itself
            await agent.Recipes.update(recipe);

            // Step 2: Handle measurements
            if (recipe.measurements && recipe.measurements.length > 0) {
                for (const measurement of recipe.measurements) {
                    if (measurement.id) {
                        // If the measurement already has an ID, update it
                        await this.updateMeasurement(measurement);
                    } else {
                        // If the measurement does not have an ID, create it
                        await this.createMeasurement(recipe.id, measurement.ingredientId, measurement.amount);
                    }
                }
            }

            runInAction(() => {
                this.recipeRegistry.set(recipe.id, recipe);
                this.selectedRecipe = recipe;
                this.editMode = false;
                this.loading = false;
            });
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            });
        }
    };

    deleteRecipe = async (id: string) => {
        this.loading = true;
        try {
            await agent.Recipes.delete(id);
            runInAction(() => {
                this.recipeRegistry.delete(id);
                /* if (this.selectedRecipe?.id === id) this.cancelSelectedRecipe();*/
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
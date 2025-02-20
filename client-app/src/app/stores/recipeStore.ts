import {makeAutoObservable, reaction, runInAction} from "mobx";
import {Measurement, Recipe, RecipeCategory} from "../models/recipe.ts";
import agent from "../api/agent.ts";
import { v4 as uuid } from "uuid";
import {Pagination, PagingParams} from "../models/pagination.ts";
import {toast} from "react-toastify";
import {store} from "./store.ts";

export default class RecipeStore {
    recipeRegistry = new Map<string, Recipe>();
    measurementRegistry = new Map<string, Measurement>(); // ✅ Store measurements separately
    selectedRecipe: Recipe | null = null;
    editMode = false;
    loading = false;
    loadingInitial = false;
    pagination: Pagination | null = null;
    pagingParams = new PagingParams();
    predicate = new Map().set('all', true);

    constructor() {
        makeAutoObservable(this);

        reaction(
            () => this.predicate.keys(),
            () => {
                this.pagingParams = new PagingParams();
                this.recipeRegistry.clear();
                this.loadRecipes();
            }
        )
    }

    setPagingParams = (pagingParams: PagingParams) => {
        this.pagingParams = pagingParams;
    }

    setPredicate = (predicate: string, value: string | RecipeCategory) => {
        const resetPredicate = () => {
            this.predicate.forEach((_value, key) => {
                this.predicate.delete(key);
            })
        }
        switch (predicate) {
            case "all":
                resetPredicate();
                this.predicate.set('all', true);
                break;
            case 'priceLow':
                resetPredicate();
                this.predicate.set('priceLow', true);
                break;
            case 'priceHigh':
                resetPredicate();
                this.predicate.set('priceHigh', true);
                break;
            case 'category':
                resetPredicate();
                this.predicate.delete('category');
                this.predicate.set('category', value.toString());
                break;
        }
    }

    get recipesLoaded() {
        return this.recipeRegistry.size > 0;
    }

    getRecipesUsingIngredient(ingredientId: string) {
        return this.recipesByCategory.filter(recipe =>
            recipe.measurements && Array.isArray(recipe.measurements) &&
            recipe.measurements.some(measurement =>
                measurement.ingredientId === ingredientId
            )
        );
    }

    clearSelectedRecipe = () => {
        runInAction(() => {
            this.selectedRecipe = null;
        });
    };

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

    get measurementsByRecipe() {
        return (recipeId: string) => {
            return Array.from(this.measurementRegistry.values()).filter(m => m.recipeId === recipeId);
        };
    }

    loadRecipes = async () => {
        this.setLoadingInitial(true);
        try {
            // Load ingredients first if they're not loaded
            if (store.ingredientStore.ingredientRegistry.size <= 1) {
                await store.ingredientStore.loadIngredients();
            }

            const result = await agent.Recipes.list(this.axiosParams);
            result.data.forEach(recipe => this.setRecipe(recipe));
            this.setPagination(result.pagination)
            this.setLoadingInitial(false);

        } catch (error) {
            console.log(error);
            runInAction(() => this.setLoadingInitial(false));
        }
    };

    get axiosParams() {
        const params = new URLSearchParams();
        params.append('pageNumber', this.pagingParams.pageNumber.toString());
        params.append('pageSize', this.pagingParams.pageSize.toString());
        this.predicate.forEach((value, key) => {
            params.append(key, value);
        })
        return params;
    }
    /*loadRecipes = async () => {
        this.setLoadingInitial(true);
        try {
            const result = await agent.Recipes.list(this.axiosParams);
            result.data.forEach(recipe => this.recipeRegistry.set(recipe.id, recipe));
            this.setLoadingInitial(false);
        } catch (error) {
            console.log(error);
            this.setLoadingInitial(false);
        }
    };*/

    setPagination = (pagination: Pagination) => {
        this.pagination = pagination;
    }

    loadMeasurements = async (recipeId: string) => {
        try {
            const measurements = await agent.Measurements.listByRecipe(recipeId);
            measurements.forEach(m => this.measurementRegistry.set(m.id, m));
        } catch (error) {
            console.log(error);
        }
    };

    loadRecipe = async (id: string) => {
        let recipe = this.getRecipe(id);
        if (recipe) {
            console.log('Recipe found in registry:', recipe);
            this.selectedRecipe = {
                ...recipe,
                measurements: recipe.measurements || [],
                nutrition: recipe.nutrition
            };
            console.log('Selected recipe after mapping:', this.selectedRecipe);
            return this.selectedRecipe;
        } else {
            this.setLoadingInitial(true);
            try {
                // Load ingredients if needed
                if (store.ingredientStore.ingredientRegistry.size <= 1) {
                    console.log('Loading ingredients before fetching recipe...');
                    await store.ingredientStore.loadIngredients();
                }

                console.log('Fetching recipe details for ID:', id);
                recipe = await agent.Recipes.details(id);
                console.log('Raw recipe data from API:', recipe);

                recipe = {
                    ...recipe,
                    measurements: recipe.measurements || [],
                    nutrition: recipe.nutrition
                };

                console.log('Processed recipe data:', recipe);
                console.log('Recipe measurements:', recipe.measurements);
                console.log('Recipe nutrition:', recipe.nutrition);

                this.setRecipe(recipe);
                runInAction(() => this.selectedRecipe = recipe || null);
                this.setLoadingInitial(false);
                return recipe;
            } catch (error) {
                console.error('Error loading recipe:', error);
                this.setLoadingInitial(false);
            }
        }
    };
/*
    loadRecipe = async (id: string) => {
        let recipe = this.getRecipe(id);
        if (recipe) {
            this.selectedRecipe = {
                ...recipe,
                measurements: recipe.measurements || [],
                nutrition: recipe.nutrition
            };
            return this.selectedRecipe;
/!*            this.selectedRecipe = {
                ...recipe,
                measurements: recipe.measurements || [],
                nutrition: recipe.nutrition || {
                    recipeId: recipe.id,
                    totalCalories: 0,
                    totalCarbs: 0,
                    totalFat: 0,
                    totalProtein: 0,
                    caloriesPerServing: 0,
                    carbsPerServing: 0,
                    fatPerServing: 0,
                    proteinPerServing: 0
                }
            };
            return this.selectedRecipe;*!/
/!*            this.selectedRecipe = {
                ...recipe,
                measurements: recipe.measurements || []
            };
            return this.selectedRecipe;*!/
        } else {
            this.setLoadingInitial(true);
            try {
                recipe = await agent.Recipes.details(id);

                recipe = {
                    ...recipe,
                    measurements: recipe.measurements || [],
                    nutrition: recipe.nutrition
                };

/!*                // Ensure full data mapping
                recipe = {
                    ...recipe,
                    measurements: recipe.measurements || [],
                    nutrition: recipe.nutrition || {
                        recipeId: recipe.id,
                        totalCalories: 0,
                        totalCarbs: 0,
                        totalFat: 0,
                        totalProtein: 0,
                        caloriesPerServing: 0,
                        carbsPerServing: 0,
                        fatPerServing: 0,
                        proteinPerServing: 0
                    }
                };*!/

                this.setRecipe(recipe);
                runInAction(() => this.selectedRecipe = recipe ?? null);
                this.setLoadingInitial(false);
                return recipe;
            } catch (error) {
                console.error('Error loading recipe:', error);
                this.setLoadingInitial(false);
            }


/!*                  recipe = await agent.Recipes.details(id);
                  // Ensure measurements is an array
                  recipe = {
                      ...recipe,
                      measurements: recipe.measurements || []
                  };
                  this.setRecipe(recipe);
                  runInAction(() => this.selectedRecipe = recipe ?? null);
                  this.setLoadingInitial(false);
                  return recipe;
              } catch (error) {
                  console.log(error);
                  this.setLoadingInitial(false);
            }*!/
        }
    };
*/

    private setRecipe = (recipe: Recipe) => {
        this.recipeRegistry.set(recipe.id, recipe);
    };

    private getRecipe = (id: string) => {
        return this.recipeRegistry.get(id);
    };

    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    };

    createMeasurement = async (recipeId: string, ingredientId: string, amount: number) => {
        try {
            const measurement: Measurement = {
                id: uuid(),
                recipeId,
                ingredientId,
                amount,
                recipeName: "",
                ingredientName: "",
                ingredientPricePerMeasurement: 0,
                ingredientCalories: 0,
                ingredientCarbs: 0,
                ingredientFat: 0,
                ingredientProtein: 0,
                pricePerAmount: 0,
                caloriesPerAmount: 0,
                carbsPerAmount: 0,
                fatPerAmount: 0,
                proteinPerAmount: 0
            };

            await agent.Measurements.create(measurement);

            // ✅ Fetch updated measurement from the API
            const updatedMeasurement = await agent.Measurements.details(measurement.id);

            runInAction(() => {
                this.measurementRegistry.set(updatedMeasurement.id, updatedMeasurement);
            });
        } catch (error) {
            console.log(error);
        }
    };

    createMeasurements = async (recipeId: string, measurements: { ingredientId: string; amount: number }[]) => {
        try {
            await Promise.all(measurements.map(m => this.createMeasurement(recipeId, m.ingredientId, m.amount)));

            // ✅ Fetch updated measurements once instead of one-by-one
            const updatedMeasurements = await agent.Measurements.list();

            runInAction(() => {
                updatedMeasurements.forEach(m => this.measurementRegistry.set(m.id, m));
                this.setLoadingInitial(false);
            });
        } catch (error) {
            console.log(error);
            this.setLoadingInitial(false);
        }
    };

    createRecipe = async (recipe: Recipe, measurements?: { ingredientId: string; amount: number }[]) => {
        this.loading = true;

        try {
            recipe.id = uuid();
            console.log('Creating recipe with ID:', recipe.id);

            // Create the basic recipe first
            await agent.Recipes.create(recipe);
            console.log('Recipe created successfully');

            // Then create measurements if they exist
            if (measurements && measurements.length > 0) {
                console.log('Creating measurements:', measurements);
                await this.createMeasurements(recipe.id, measurements);
            }

            // Set the basic recipe in the store
            runInAction(() => {
                this.recipeRegistry.set(recipe.id, recipe);
                this.selectedRecipe = null;
                this.editMode = false;
                this.setLoadingInitial(false);
                this.loading = false;
            });

            return recipe;
        } catch (error) {
            console.error('Error creating recipe:', error);
            runInAction(() => {
                this.loading = false;
            });
            throw error;
        }
    };
/*    createRecipe = async (recipe: Recipe, measurements?: { ingredientId: string; amount: number }[]) => {
        this.loading = true;

        try {
            recipe.id = uuid();
            console.log('Creating recipe with ID:', recipe.id);

            // First create the recipe
            await agent.Recipes.create(recipe);
            console.log('Recipe created successfully');

            // Create measurements if they exist
            if (measurements && measurements.length > 0) {
                console.log('Creating measurements:', measurements);
                await this.createMeasurements(recipe.id, measurements);
            }

            // Add a longer delay here
            console.log('Waiting for database consistency...');
            await new Promise(resolve => setTimeout(resolve, 1000));

            // Try loading several times
            let fullRecipe = null;
            for(let i = 0; i < 3; i++) {
                try {
                    console.log(`Attempt ${i + 1} to load recipe...`);
                    fullRecipe = await agent.Recipes.details(recipe.id);
                    if(fullRecipe) break;
                } catch(e) {
                    console.log(`Attempt ${i + 1} failed, retrying...`);
                    await new Promise(resolve => setTimeout(resolve, 500));
                }
            }

            if(fullRecipe) {
                runInAction(() => {
                    this.recipeRegistry.set(recipe.id, fullRecipe);
                    this.selectedRecipe = fullRecipe;
                    this.editMode = false;
                    this.loading = false;
                });
                return fullRecipe;
            } else {
                throw new Error('Could not load recipe after creation');
            }
        } catch (error) {
            console.error('Error creating/loading recipe:', error);
            runInAction(() => {
                this.loading = false;
            });
            throw error;
        }
    };*/
/*
    createRecipe = async (recipe: Recipe, measurements?: { ingredientId: string; amount: number }[]) => {
        this.loading = true;

        try {
            recipe.id = uuid();
            await agent.Recipes.create(recipe);

            runInAction(() => {
                this.recipeRegistry.set(recipe.id, recipe);
                this.selectedRecipe = null;
                this.editMode = false;
                this.loading = false;
            });

            if (measurements && measurements.length > 0) {
                await this.createMeasurements(recipe.id, measurements);
            }

            runInAction(() => {
                this.editMode = false;
                this.setLoadingInitial(false);
            });
        } catch (error) {
            console.error(error);
            runInAction(() => {
                this.loading = false;
            });
        }
    };
*/

    updateMeasurement = async (measurement: Measurement) => {
        try {
            await agent.Measurements.update(measurement);
            runInAction(() => {
                this.measurementRegistry.set(measurement.id, measurement);
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
            const updatedMeasurements = await agent.Measurements.list();
            runInAction(() => {
                updatedMeasurements.forEach(m => this.measurementRegistry.set(m.id, m));
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


    deleteRecipe = async (id: string): Promise<boolean> => {
        if (window.confirm("Are you sure you want to delete this recipe?")) {
            this.loading = true;
            try {
                await agent.Recipes.delete(id);
                runInAction(() => {
                    this.recipeRegistry.delete(id);
                    this.loading = false;
                });
                toast.success("Recipe deleted successfully");
                return true;
            } catch (error) {
                console.log(error);
                runInAction(() => {
                    this.loading = false;
                });
                toast.error("Problem deleting recipe");
                return false;
            }
        }
        return false;
    };

  /*  deleteRecipe = async (id: string) => {
        this.loading = true;
        try {
            await agent.Recipes.delete(id);
            runInAction(() => {
                this.recipeRegistry.delete(id);
                this.loading = false;
            });
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            });
        }
    };*/
}

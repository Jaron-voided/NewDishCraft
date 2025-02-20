import {makeAutoObservable, reaction, runInAction} from "mobx";
import {Ingredient, IngredientCategory, MeasuredIn} from "../models/ingredient.ts";
import agent from "../api/agent.ts";
import {v4 as uuid} from 'uuid';
import {Pagination, PagingParams} from "../models/pagination.ts";
import {store} from "./store.ts";
import {toast} from "react-toastify";

export default class IngredientStore {

    ingredientRegistry = new Map<string, Ingredient>();
    selectedIngredient: Ingredient | null = null;
    editMode = false;
    loading = false;
    loadingInitial = false;
    pagination: Pagination | null = null;
    pagingParams = new PagingParams();
    predicate = new Map().set('all', true);


    constructor() {
        makeAutoObservable(this)

        reaction(
            () => this.predicate.keys(),
            () => {
                this.pagingParams = new PagingParams();
                this.ingredientRegistry.clear();
                this.loadIngredients();
            }
        )
    }

    setPagingParams = (pagingParams: PagingParams) => {
        this.pagingParams = pagingParams;
    }

    setPredicate = (predicate: string, value: string | IngredientCategory) => {
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

    get axiosParams() {
        const params = new URLSearchParams();
        params.append('pageNumber', this.pagingParams.pageNumber.toString());
        params.append('pageSize', this.pagingParams.pageSize.toString());
        this.predicate.forEach((value, key) => {
            params.append(key, value);
        })
        return params;
    }

    get ingredientsByPrice() {
        return Array.from(this.ingredientRegistry.values()).sort((a, b) =>
            (a.pricePerMeasurement ?? 0) - (b.pricePerMeasurement ?? 0)
        );
    }


    /*    get ingredientsByPrice() {
            return Array.from(this.ingredientRegistry.values()).sort((a, b) => a.pricePerMeasurement - b.pricePerMeasurement);
        }*/

    get ingredientsByCategory() {
        return Array.from(this.ingredientRegistry.values()).sort((a, b) =>
            a.category.localeCompare(b.category)
        );
    }

    get ingredientsLoaded() {
        return this.ingredientRegistry.size > 0;
    }

    /*
        get ingredientsByCategory() {
            return Array.from(this.ingredientRegistry.values()).sort((a, b) =>
                Number(a.category) - Number(b.category)
            );
        }
    */

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
            const result = await agent.Ingredients.list(this.axiosParams);
            console.log('Raw Ingredients API Result:', result);

            if (result.data && result.data.length > 0) {
                result.data.forEach(ingredient => {
                    console.group(`Ingredient: ${ingredient.name}`);
                    console.log('Ingredient Object Type:', typeof ingredient);
                    console.log('Ingredient Object Keys:', Object.keys(ingredient));
                    console.log('Full Ingredient Object:', JSON.stringify(ingredient, null, 2));
                    console.log('Nutrition Object Raw:', ingredient.nutrition);

                    // Try to access nutrition properties explicitly
                    console.log('Nutrition Access Test:', {
                        calories: ingredient['nutrition']?.calories,
                        protein: ingredient['nutrition']?.protein,
                        carbs: ingredient['nutrition']?.carbs,
                        fat: ingredient['nutrition']?.fat
                    });
                    console.groupEnd();

                    this.setIngredient(ingredient);
                });
            } else {
                console.log('No ingredients returned');
            }

            this.setPagination(result.pagination)
            this.setLoadingInitial(false);

        } catch (error) {
            console.error('Error loading ingredients:', error);
            runInAction(() => this.setLoadingInitial(false));
        }
    };
   /* loadIngredients = async () => {
        this.setLoadingInitial(true);
        try {
            const result = await agent.Ingredients.list(this.axiosParams);
            console.log('Full Ingredients API Result:', result);

            if (result.data && result.data.length > 0) {
                result.data.forEach(ingredient => {
                    console.group(`Ingredient: ${ingredient.name}`);
                    console.log('Full Ingredient Object:', ingredient);
                    console.log('Nutrition Object:', ingredient.nutrition);
                    console.log('Nutrition Details:', {
                        calories: ingredient.nutrition?.calories,
                        protein: ingredient.nutrition?.protein,
                        carbs: ingredient.nutrition?.carbs,
                        fat: ingredient.nutrition?.fat
                    });
                    console.groupEnd();

                    this.setIngredient(ingredient);
                });
            } else {
                console.log('No ingredients returned');
            }

            this.setPagination(result.pagination)
            this.setLoadingInitial(false);

        } catch (error) {
            console.error('Error loading ingredients:', error);
            runInAction(() => this.setLoadingInitial(false));
        }
    };*/
/*    loadIngredients = async () => {
        this.setLoadingInitial(true);
        try {
            const result = await agent.Ingredients.list(this.axiosParams);
            result.data.forEach(ingredient => this.setIngredient(ingredient));
            this.setPagination(result.pagination)
            this.setLoadingInitial(false);

        } catch (error) {
            console.log(error);
            runInAction(() => this.setLoadingInitial(false));
        }
    };*/

    setPagination = (pagination: Pagination) => {
        this.pagination = pagination;
    }

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
                runInAction(() => this.selectedIngredient = ingredient ?? null);
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

    getIngredient = (id: string) => {
        return this.ingredientRegistry.get(id);
    }


    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    createIngredient = async (ingredient: Ingredient) => {
        this.loading = true;

        try {
            ingredient.id = uuid();

            await agent.Ingredients.create({
                ...ingredient,
                measurementUnit: {
                    ingredientId: ingredient.id,
                    measuredIn: ingredient.measurementUnit.measuredIn || MeasuredIn.Weight,
                    weightUnit: ingredient.measurementUnit.weightUnit,
                    volumeUnit: ingredient.measurementUnit.volumeUnit
                },
                nutrition: {
                    ingredientId: ingredient.id,
                    calories: ingredient.nutrition?.calories || 0,
                    protein: ingredient.nutrition?.protein || 0,
                    carbs: ingredient.nutrition?.carbs || 0,
                    fat: ingredient.nutrition?.fat || 0
                }
            });

            runInAction(() => {
                this.ingredientRegistry.set(ingredient.id, ingredient);
                this.selectedIngredient = null;
                this.editMode = false;
                this.loading = false;
            });
        } catch (error) {
            console.error(error);
            runInAction(() => {
                this.loading = false;
            });
        }
    };

    updateIngredient = async (ingredient: Ingredient) => {
        this.loading = true;
        try {
            await agent.Ingredients.update({
                ...ingredient,
                measurementUnit: {
                    ingredientId: ingredient.id,
                    measuredIn: ingredient.measurementUnit.measuredIn,
                    weightUnit: ingredient.measurementUnit.weightUnit,
                    volumeUnit: ingredient.measurementUnit.volumeUnit
                },
                nutrition: {
                    ingredientId: ingredient.id,
                    calories: ingredient.nutrition?.calories || 0,
                    protein: ingredient.nutrition?.protein || 0,
                    carbs: ingredient.nutrition?.carbs || 0,
                    fat: ingredient.nutrition?.fat || 0
                }
            });

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

/*    createIngredient = async (ingredient: Ingredient) => {
        this.loading = true;

        try {
            ingredient.id = uuid();

            await agent.Ingredients.create({
                id: ingredient.id,
                name: ingredient.name,
                category: ingredient.category, // ✅ Already a string
                measuredIn: ingredient.measuredIn, // ✅ Already a string
                pricePerPackage: ingredient.pricePerPackage,
                measurementsPerPackage: ingredient.measurementsPerPackage,
                calories: ingredient.calories, // ✅ Directly sending nutritional values
                carbs: ingredient.carbs,
                fat: ingredient.fat,
                protein: ingredient.protein
            });

            runInAction(() => {
                this.ingredientRegistry.set(ingredient.id, ingredient);
                this.selectedIngredient = ingredient;
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
    }*/

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

    deleteIngredientWithConfirmation = async (id: string): Promise<boolean> => {
        // Access recipeStore through the imported store
        const recipesUsingIngredient = store.recipeStore.getRecipesUsingIngredient(id);

        let confirmMessage = "Are you sure you want to delete this ingredient?";
        if (recipesUsingIngredient.length > 0) {
            confirmMessage = `This ingredient is used in ${recipesUsingIngredient.length} recipe${recipesUsingIngredient.length > 1 ? 's' : ''} which will also be affected. Are you sure you want to delete it?`;
        }

        if (window.confirm(confirmMessage)) {
            this.loading = true;
            try {
                await agent.Ingredients.delete(id);
                runInAction(() => {
                    this.ingredientRegistry.delete(id);
                    this.loading = false;
                });
                toast.success("Ingredient deleted successfully");
                return true;
            } catch (error) {
                console.log(error);
                runInAction(() => {
                    this.loading = false;
                });
                toast.error("Problem deleting ingredient");
                return false;
            }
        }
        return false;
    };
}
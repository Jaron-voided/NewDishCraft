import {makeAutoObservable, reaction, runInAction} from "mobx";
import {Measurement, Recipe} from "../models/recipe.ts";
import agent from "../api/agent.ts";
import { v4 as uuid } from "uuid";
import {Pagination, PagingParams} from "../models/pagination.ts";
import {DayPlan, DayPlanRecipe} from "../models/dayPlan.ts";
import {Ingredient} from "../models/ingredient.ts";

export default class DayPlanStore {
    dayPlanRegistry = new Map<string, DayPlan>();
    dayPlanRecipeRegistry = new Map<string, DayPlanRecipe>();
    recipeRegistry = new Map<string, Recipe>();
    ingredientRegistry = new Map<string, Ingredient>();
    measurementRegistry = new Map<string, Measurement>(); //
    selectedDayPlan: DayPlan | undefined = undefined;
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
                this.dayPlanRegistry.clear();
                this.loadDayPlans();
            }
        )
    }

    setPagingParams = (pagingParams: PagingParams) => {
        this.pagingParams = pagingParams;
    }

 /*   setPredicate = (predicate: string, value: string | DayPlanCategory) => {
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
*/

    get dayPlans() {
        return Array.from(this.dayPlanRegistry.values());
    }

    get groupedDayPlans() {
        return Object.entries(
            this.dayPlans.reduce((dayPlans/*, dayPlan*/) => {
                return dayPlans;
            }, {} as Record<string, DayPlan[]>)
        );
    }

    get measurementsByRecipe() {
        return (recipeId: string) => {
            return Array.from(this.measurementRegistry.values()).filter(m => m.recipeId === recipeId);
        };
    }

    loadDayPlans = async () => {
        this.setLoadingInitial(true);
        try {
            const result = await agent.DayPlans.list(this.axiosParams);
            result.data.forEach(dayPlan => this.setDayPlan(dayPlan));
            this.setPagination(result.pagination);
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

    setPagination = (pagination: Pagination) => {
        this.pagination = pagination;
    }

    loadMeasurements = async () => {
        try {
            const measurements = await agent.Measurements.list();
            measurements.forEach(m => this.measurementRegistry.set(m.id, m));
        } catch (error) {
            console.log(error);
        }
    };

    loadDayPlan = async (id: string) => {
        let dayPlan = this.getDayPlan(id);
        if (dayPlan) {
            this.selectedDayPlan = dayPlan;
            return dayPlan;
        } else {
            this.setLoadingInitial(true);
            try {
                dayPlan = await agent.DayPlans.details(id);
                this.setDayPlan(dayPlan);
                runInAction(() => this.selectedDayPlan = dayPlan);
                this.setLoadingInitial(false);
                return dayPlan;
            } catch (error) {
                console.log(error);
                this.setLoadingInitial(false);
            }
        }
    };

    private setDayPlan = (dayPlan: DayPlan) => {
        this.dayPlanRegistry.set(dayPlan.id, dayPlan);
    };

    private getDayPlan = (id: string) => {
        return this.dayPlanRegistry.get(id);
    };

    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    };

    createDayPlanRecipe = async (dayPlanId: string, recipeId: string, servings: number) => {
        try {
            const dayPlanRecipe: DayPlanRecipe = {
                id: uuid(),
                dayPlanId,
                recipeId,
                recipeName: "",
                servings,

                pricePerServing: 0,
                caloriesPerRecipe: 0,
                carbsPerRecipe: 0,
                proteinPerRecipe: 0,
                fatPerRecipe: 0,
                servingsPerRecipe: 0
            };

            await agent.DayPlanRecipes.create(dayPlanRecipe);

            // ✅ Fetch updated dayPlanRecipe from the API
            const updatedDayPlanRecipe = await agent.DayPlanRecipes.details(dayPlanRecipe.id);

            runInAction(() => {
                this.dayPlanRecipeRegistry.set(updatedDayPlanRecipe.id, updatedDayPlanRecipe);
            });
        } catch (error) {
            console.log(error);
        }
    };

    createDayPlanRecipes = async (dayPlanId: string, dayPlanRecipes: { recipeId: string; servings: number }[]) => {
        try {
            await Promise.all(dayPlanRecipes.map(dpr => this.createDayPlanRecipe(dayPlanId, dpr.recipeId, dpr.servings)));

            // ✅ Fetch updated dayPlanRecipes once instead of one-by-one
            const updatedDayPlanRecipes = await agent.DayPlanRecipes.list();

            runInAction(() => {
                updatedDayPlanRecipes.forEach(dpr => this.dayPlanRecipeRegistry.set(dpr.id, dpr));
                this.setLoadingInitial(false);
            });
        } catch (error) {
            console.log(error);
            this.setLoadingInitial(false);
        }
    };

    createDayPlan = async (dayPlan: DayPlan, dayPlanRecipes?: { recipeId: string; servings: number }[]) => {
        this.loading = true;

        try {
            dayPlan.id = uuid();
            await agent.DayPlans.create(dayPlan);

            runInAction(() => {
                this.dayPlanRegistry.set(dayPlan.id, dayPlan);
                this.selectedDayPlan = dayPlan;
            });

            if (dayPlanRecipes && dayPlanRecipes.length > 0) {
                await this.createDayPlanRecipes(dayPlan.id, dayPlanRecipes);
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

    updateDayPlanRecipe = async (dayPlanRecipe: DayPlanRecipe) => {
        try {
            await agent.DayPlanRecipes.update(dayPlanRecipe);
            runInAction(() => {
                this.dayPlanRecipeRegistry.set(dayPlanRecipe.id, dayPlanRecipe);
            });
        } catch (error) {
            console.log(error);
        }
    };

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

    updateDayPlan = async (dayPlan: DayPlan) => {
        this.loading = true;

        try {
            // Step 1: Update the dayPlan itself
            await agent.DayPlans.update(dayPlan);

            // Step 2: Handle dayPlanRecipes
            const updatedDayPlanRecipes = await agent.DayPlanRecipes.list();
            runInAction(() => {
                updatedDayPlanRecipes.forEach(m => this.dayPlanRecipeRegistry.set(m.id, m));
                this.dayPlanRegistry.set(dayPlan.id, dayPlan);
                this.selectedDayPlan = dayPlan;
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

    deleteDayPlan = async (id: string) => {
        this.loading = true;
        try {
            await agent.DayPlans.delete(id);
            runInAction(() => {
                this.dayPlanRegistry.delete(id);
                this.loading = false;
            });
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            });
        }
    };
}

import axios, {AxiosError, AxiosResponse} from "axios";
import {Ingredient} from "../models/ingredient.ts";
import {Measurement, Recipe} from "../models/recipe.ts";
import {toast} from "react-toastify";
import {router} from "../router/Routes.tsx";
import {store} from "../stores/store.ts";
import {User, UserFormValues} from "../models/user.ts";
import {PaginatedResult} from "../models/pagination.ts";
import {DayPlan, DayPlanCalculationsDto, DayPlanRecipe} from "../models/dayPlan.ts";


const sleep = (delay: number) => {
    return new Promise(resolve => {
        setTimeout(resolve, delay);
    })
}

axios.defaults.baseURL = 'http://localhost:5000/api';

const responseBody = <T> (response: AxiosResponse<T>) => response.data;

axios.interceptors.request.use(config => {
    const token = store.commonStore.token;
    if (token && config.headers) config.headers.Authorization = `Bearer ${token}`;
    return config;
})

axios.interceptors.response.use(async response => {
/*    try {
        await sleep(1000);
        return response;
    } catch (error) {
        console.log(error);
        return Promise.reject(error);
    }*/
    await sleep(1000);
    const pagination = response.headers['pagination'];
    if (pagination) {
        response.data = new PaginatedResult(response.data, JSON.parse(pagination));
        return response as AxiosResponse<PaginatedResult<unknown>>
    }
    return response;
}, (error: AxiosError) => {
        const {data, status, config} = error.response as AxiosResponse;
        switch (status) {
            case 400:
                if (config.method === 'get' && Object.prototype.hasOwnProperty.call(data.errors, 'id')) {
                    router.navigate('/not-found')
                }
                if (data.errors) {
                    const modalStateErrors = [];
                    for (const key in data.errors) {
                        if (data.errors[key]) {
                            modalStateErrors.push(data.errors[key]);
                        }
                    }
                    throw modalStateErrors.flat();
                } else {
                    toast.error(data);
                }
                break;
            case 401:
                toast.error('Unauthorized');
                break;
            case 403:
                toast.error('Forbidden');
                break;
            case 404:
                router.navigate('/not-found')
                break;
            case 500:
                store.commonStore.setServerError(data);
                router.navigate('/server-error');
                break;
    }
    return Promise.reject(error);
})


const requests = {
    get: <T> (url: string) => axios.get<T>(url).then(responseBody),
    post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
}

const Ingredients = {
    list: (params: URLSearchParams) => {
        return axios.get<PaginatedResult<Ingredient[]>>('/ingredients', {params})
            .then(response => {
                console.log('Raw API Response:', response);
                return responseBody(response);
            });
    },
    details: (id: string) => {
        return requests.get<Ingredient>(`/ingredients/${id}`)
            .then(response => {
                console.log('Raw Details Response:', response);
                return response;
            });
    },
/*    list: (params: URLSearchParams) => axios.get<PaginatedResult<Ingredient[]>>('/ingredients', {params})
        .then(responseBody),
    details: (id: string) => requests.get<Ingredient>(`/ingredients/${id}`),*/
    create: (ingredient: Ingredient) => axios.post<void>('/ingredients', ingredient),
    update: (ingredient: Ingredient) => axios.put<void>(`/ingredients/${ingredient.id}`, ingredient),
    delete: (id: string) => axios.delete<void>(`/ingredients/${id}`)
}
const Measurements = {
    list: () => requests.get<Measurement[]>('/measurements'),
    listByRecipe: (recipeId: string) => requests.get<Measurement[]>(`/measurements/by-recipe/${recipeId}`),
    details: (id: string) => requests.get<Measurement>(`/measurements/${id}`),
    create: (measurement: Measurement) => axios.post<void>('/measurements', measurement),
    update: (measurement: Measurement) => axios.put<void>(`/measurements/${measurement.id}`, measurement),
    delete: (id: string) => axios.delete<void>(`/measurements/${id}`)
}

const Recipes = {
    list: (params: URLSearchParams) => axios.get<PaginatedResult<Recipe[]>>('/recipes',  {params})
        .then(responseBody),
    //list: () => requests.get<Recipe[]>('/recipes'),
    details: (id: string) => requests.get<Recipe>(`/recipes/${id}`).catch(error => {
        if(error.response?.status === 404) {
            return null;
        }
        throw error;
    }),
    create: (recipe: Recipe) => axios.post<void>('/recipes', recipe),
    update: (recipe: Recipe) => axios.put<void>(`/recipes/${recipe.id}`, recipe),
    delete: (id: string) => axios.delete<void>(`/recipes/${id}`)
}

const DayPlanRecipes = {
    list: () => requests.get<DayPlanRecipe[]>('/recipes'),
    //list: () => requests.get<DayPlanRecipe[]>('/recipes'),
    details: (id: string) => requests.get<DayPlanRecipe>(`/dayPlanRecipes/${id}`),
    create: (dayPlanRecipe: DayPlanRecipe) => axios.post<void>('/dayPlanRecipes', dayPlanRecipe),
    update: (dayPlanRecipe: DayPlanRecipe) => axios.put<void>(`/dayPlanRecipes/${dayPlanRecipe.id}`, dayPlanRecipe),
    delete: (id: string) => axios.delete<void>(`/dayPlanRecipes/${id}`)
}

const DayPlans = {
    list: (params: URLSearchParams) => axios.get<PaginatedResult<DayPlan[]>>('/dayPlan',  {params})
        .then(responseBody),
    //list: () => requests.get<DayPlan[]>('/dayPlan'),
    details: (id: string) => requests.get<DayPlan>(`/dayPlan/${id}`),
    create: (dayPlan: DayPlan) => axios.post<void>('/dayPlan', dayPlan),
    update: (dayPlan: DayPlan) => axios.put<void>(`/dayPlan/${dayPlan.id}`, dayPlan),
    delete: (id: string) => axios.delete<void>(`/dayPlan/${id}`),
    calculations: (id: string) => axios.get<DayPlanCalculationsDto>(`/dayPlan/${id}/calculations`)
}

const Account = {
    current: () => requests.get<User>('/account'),
    login: (user: UserFormValues) => requests.post<User>('/account/login', user),
    register: (user: UserFormValues) => requests.post<User>('/account/register', user),
}

const agent = {
    Ingredients,
    Recipes,
    Measurements,
    DayPlans,
    DayPlanRecipes,
    Account
}

export default agent;
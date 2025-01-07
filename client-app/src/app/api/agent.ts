import axios, {AxiosError, AxiosResponse} from "axios";
import {Ingredient} from "../models/ingredient.ts";
import {Measurement, Recipe} from "../models/recipe.ts";
import {toast} from "react-toastify";
import {router} from "../router/Routes.tsx";
import {store} from "../stores/store.ts";


const sleep = (delay: number) => {
    return new Promise(resolve => {
        setTimeout(resolve, delay);
    })
}

axios.defaults.baseURL = 'http://localhost:5000/api';

axios.interceptors.response.use(async response => {
/*    try {
        await sleep(1000);
        return response;
    } catch (error) {
        console.log(error);
        return Promise.reject(error);
    }*/
    await sleep(1000);
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

const responseBody = <T> (response: AxiosResponse<T>) => response.data;

const requests = {
    get: <T> (url: string) => axios.get<T>(url).then(responseBody),
    post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
}

const Ingredients = {
    list: () => requests.get<Ingredient[]>('/ingredients'),
    details: (id: string) => requests.get<Ingredient>(`/ingredients/${id}`),
    create: (ingredient: Ingredient) => axios.post<void>('/ingredients', ingredient),
    update: (ingredient: Ingredient) => axios.put<void>(`/ingredients/${ingredient.id}`, ingredient),
    delete: (id: string) => axios.delete<void>(`/ingredients/${id}`)
}
const Measurements = {
    list: () => requests.get<Measurement[]>('/measurements'),
    details: (id: string) => requests.get<Measurement>(`/measurements/${id}`),
    create: (measurement: Measurement) => axios.post<void>('/measurements', measurement),
    update: (measurement: Measurement) => axios.put<void>(`/measurements/${measurement.id}`, measurement),
    delete: (id: string) => axios.delete<void>(`/measurements/${id}`)
}

const Recipes = {
    list: () => requests.get<Recipe[]>('/recipes'),
    details: (id: string) => requests.get<Recipe>(`/recipes/${id}`),
    create: (recipe: Recipe) => axios.post<void>('/recipes', recipe),
    update: (recipe: Recipe) => axios.put<void>(`/recipes/${recipe.id}`, recipe),
    delete: (id: string) => axios.delete<void>(`/recipes/${id}`)
}

const agent = {
    Ingredients,
    Recipes,
    Measurements
}

export default agent;
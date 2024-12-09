import axios, {AxiosResponse} from "axios";
import {Ingredient} from "../models/ingredient.ts";


const sleep = (delay: number) => {
    return new Promise(resolve => {
        setTimeout(resolve, delay);
    })
}

axios.defaults.baseURL = 'http://localhost:5000/api';

axios.interceptors.response.use(async response => {
    try {
        await sleep(1000);
        return response;
    } catch (error) {
        console.log(error);
        return Promise.reject(error);
    }
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

const agent = {
    Ingredients
}

export default agent;
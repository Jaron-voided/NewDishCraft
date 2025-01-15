import {createBrowserRouter, Navigate, RouteObject} from "react-router-dom";
import App from "../layout/App.tsx";
import HomePage from "../../features/home/HomePage.tsx";
import IngredientDashboard from "../../features/ingredients/dashboard/IngredientDashboard.tsx";
import IngredientForm from "../../features/ingredients/form/IngredientForm.tsx";
import IngredientDetails from "../../features/ingredients/details/IngredientDetails.tsx";
import RecipeDashboard from "../../features/recipes/dashboard/RecipeDashboard.tsx";
import RecipeDetails from "../../features/recipes/details/RecipeDetails.tsx";
import RecipeForm from "../../features/recipes/form/RecipeForm.tsx";
import TestErrors from "../../features/errors/TestError.tsx";
import NotFound from "../../features/errors/NotFound.tsx";
import ServerError from "../../features/errors/ServerError.tsx";
import LoginForm from "../../features/users/LoginForm.tsx";

export const routes: RouteObject[] = [
    {
        path: '/',
        element: <App />,
        children: [
            {path: '', element: <HomePage />},

            /*Ingredients*/
            {path: 'ingredients', element: <IngredientDashboard />},
            {path: 'ingredients/:id', element: <IngredientDetails />},
            {path: 'createIngredient', element: <IngredientForm key='create'/>},
            {path: 'manage/:id', element: <IngredientForm key='manage'/>},
            {path: 'login', element: <LoginForm />},
            {path: 'errors', element: <TestErrors />},
            {path: 'not-found', element: <NotFound />},
            {path: 'server-error', element: <ServerError />},
            {path: '*', element: <Navigate replace to='/not-found' />},

            /*Recipes*/
            {path: 'recipes', element: <RecipeDashboard />},
            {path: 'recipes/:id', element: <RecipeDetails />},
            {path: 'createRecipe', element: <RecipeForm key='create'/>},
            {path: 'manage/:id', element: <RecipeForm key='manage'/>},


        ]
    }
]

export const router = createBrowserRouter(routes)
import {createBrowserRouter, Navigate, RouteObject} from "react-router-dom";
import App from "../layout/App.tsx";
import HomePage from "../../features/home/HomePage.tsx";
import IngredientDashboard from "../../features/ingredients/dashboard/IngredientDashboard.tsx";
import IngredientForm from "../../features/ingredients/form/IngredientForm.tsx";
import RecipeDashboard from "../../features/recipes/dashboard/RecipeDashboard.tsx";
import RecipeDetails from "../../features/recipes/details/RecipeDetails.tsx";
import RecipeForm from "../../features/recipes/form/RecipeForm.tsx";
import TestErrors from "../../features/errors/TestError.tsx";
import ServerError from "../../features/errors/ServerError.tsx";
import LoginForm from "../../features/users/LoginForm.tsx";
import UserProfile from "../../features/users/UserProfile.tsx";
import DayPlanDashboard from "../../features/dayPlans/dashboard/DayPlanDashboard.tsx";
import DayPlanDetails from "../../features/dayPlans/details/DayPlanDetails.tsx";
import DayPlanForm from "../../features/dayPlans/form/DayPlanForm.tsx";

export const routes: RouteObject[] = [
    {
        path: '/',
        element: <App />,
        children: [

            /*HomePage*/
            {path: '', element: <HomePage />},

            /*User*/
            {path: 'login', element: <LoginForm />},
            {path: '/profile/:username', element: <UserProfile />},

            /*Ingredients*/
            {path: 'ingredients', element: <IngredientDashboard />},
            //{path: 'ingredients/:id', element: <IngredientDetails />},
            {path: 'createIngredient', element: <IngredientForm key='create'/>},
            {path: 'ingredients/manage/:id', element: <IngredientForm key='manage'/>},

            /*Recipes*/
            {path: 'recipes', element: <RecipeDashboard />},
            {path: 'recipes/:id', element: <RecipeDetails />},
            {path: 'createRecipe', element: <RecipeForm key='create'/>},
            {path: 'recipes/manage/:id', element: <RecipeForm key='manage'/>},
            
            /*DayPlans*/
            {path: 'dayPlan', element: <DayPlanDashboard />},
            {path: 'dayPlan/:id', element: <DayPlanDetails />},
            {path: 'createDayPlan', element: <DayPlanForm key='create'/>},
            {path: 'dayPlan/manage/:id', element: <DayPlanForm key='manage'/>},

            /*Errors*/
            {path: 'errors', element: <TestErrors />},
            {path: 'server-error', element: <ServerError />},
            {path: '*', element: <Navigate replace to='/not-found' />},
        ]
    }
]

export const router = createBrowserRouter(routes)
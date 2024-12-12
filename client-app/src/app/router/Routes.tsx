import {createBrowserRouter, RouteObject} from "react-router-dom";
import App from "../layout/App.tsx";
import HomePage from "../../features/home/HomePage.tsx";
import IngredientDashboard from "../../features/ingredients/dashboard/IngredientDashboard.tsx";
import IngredientForm from "../../features/ingredients/form/IngredientForm.tsx";
import IngredientDetails from "../../features/ingredients/details/IngredientDetails.tsx";

export const routes: RouteObject[] = [
    {
        path: '/',
        element: <App />,
        children: [
            {path: '', element: <HomePage />},
            {path: 'ingredients', element: <IngredientDashboard />},
            {path: 'ingredients/:id', element: <IngredientDetails />},
            {path: 'createIngredient', element: <IngredientForm key='create'/>},
            {path: 'manage/:id', element: <IngredientForm key='manage'/>},

        ]
    }
]

export const router = createBrowserRouter(routes)
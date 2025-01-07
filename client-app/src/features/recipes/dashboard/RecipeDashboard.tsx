import {observer} from "mobx-react-lite";
import {useStore} from "../../../app/stores/store.ts";
import {useEffect} from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";
import {Grid} from "semantic-ui-react";
import RecipeFilters from "./RecipeFilters.tsx";
import RecipeList from "./RecipeList.tsx";


export default observer(function RecipeDashboard() {
    const {recipeStore} = useStore();
    const {loadRecipes, recipeRegistry} = recipeStore;

    useEffect(() => {
        if (recipeRegistry.size <= 1) loadRecipes();

    }, [loadRecipes, recipeRegistry.size]);

    if (recipeStore.loadingInitial) return <LoadingComponent content='Loading...' />

    return (
        <Grid>
            <Grid.Column width='10'>
                <RecipeList />
            </Grid.Column>
            <Grid.Column width='6'>
                <RecipeFilters />
            </Grid.Column>
        </Grid>
    )
})

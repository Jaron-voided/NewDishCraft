import {Grid} from "semantic-ui-react";
import {useStore} from "../../../app/stores/store.ts";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";
import {observer} from "mobx-react-lite";
import {useParams} from "react-router-dom";
import {useEffect} from "react";
import RecipeDetailedInfo from "./RecipeDetailedInfo.tsx";
import RecipeDetailedHeader from "./RecipeDetailedHeader.tsx";
import RecipeDetailedSidebar from "./RecipeDetailedSidebar.tsx";



export default observer(function RecipeDetails() {
    const {recipeStore} = useStore();
    const {selectedRecipe: recipe, loadRecipe, loadingInitial} = recipeStore;
    const {id} = useParams();

    useEffect(() => {
        if (id) loadRecipe(id);
    }, [id, loadRecipe]);

    if (loadingInitial || !recipe) return <LoadingComponent />;

    return (
        <Grid>
            <Grid.Column width={10}>
                <RecipeDetailedHeader recipe={recipe} />
                <RecipeDetailedInfo recipe={recipe}/>
            </Grid.Column>
            <Grid.Column width={6}>
                <RecipeDetailedSidebar />
            </Grid.Column>
        </Grid>
    )
})
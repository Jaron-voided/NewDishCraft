import {Grid} from "semantic-ui-react";
import {useStore} from "../../../app/stores/store.ts";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";
import {observer} from "mobx-react-lite";
import {useParams} from "react-router-dom";
import {useEffect} from "react";
import RecipeDetailedInfo from "./RecipeDetailedInfo.tsx";
import RecipeDetailedHeader from "./RecipeDetailedHeader.tsx";
import RecipeDetailedSidebar from "./RecipeDetailedSidebar.tsx";
import {expandMeasurements} from "../../../app/Utilities/expandMeasurements.ts";



export default observer(function RecipeDetails() {
    const {recipeStore, ingredientStore} = useStore();
    const {selectedRecipe: recipe, loadRecipe, loadingInitial} = recipeStore;
    const {id} = useParams();

    useEffect(() => {
        if (id) {
            console.log('Loading recipe:', id);
            if (ingredientStore.ingredientRegistry.size == 0) {
                ingredientStore.loadIngredients().then(() => {
                    // Only load recipe after ingredients are loaded
                    loadRecipe(id)
                    console.log('Recipe data:', recipe);
                    console.log('Recipe measurements:', recipe?.measurements);
                    console.log('Recipe nutrition:', recipe?.nutrition);
                });
            } else {
                loadRecipe(id)
                console.log('Recipe data:', recipe);
                console.log('Recipe measurements:', recipe?.measurements);
                console.log('Recipe nutrition:', recipe?.nutrition);
            }
        }
    }, [id, loadRecipe]);

    if (loadingInitial || !recipe) return <LoadingComponent />;

    const fullMeasurements = recipe.measurements || null;
    //const fullMeasurements = expandMeasurements(recipe, ingredientStore);

    return (
        <Grid>
            <Grid.Column width={10}>
                <RecipeDetailedHeader recipe={recipe} />
                <RecipeDetailedInfo recipe={recipe}/>
            </Grid.Column>
            <Grid.Column width={6}>
                <RecipeDetailedSidebar measurements={fullMeasurements} />
            </Grid.Column>
        </Grid>
    )
})
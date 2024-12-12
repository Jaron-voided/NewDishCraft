import {Grid} from "semantic-ui-react";
import IngredientList from "./IngredientList.tsx";

import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";
import {useEffect} from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";


export default observer(function IngredientDashboard() {

    const {ingredientStore} = useStore();
    const {loadIngredients, ingredientRegistry} = ingredientStore;

    useEffect(() => {
        if (ingredientRegistry.size <= 1) loadIngredients();
/*        if (ingredientStore.ingredientsByPrice.length === 0) {
            loadIngredients();
        }*/
    }, [loadIngredients, ingredientRegistry.size]);

    if (ingredientStore.loadingInitial) return <LoadingComponent content='Loading...' />

    return (
        <Grid>
           <Grid.Column width='10'>
                <IngredientList />
           </Grid.Column>
            <Grid.Column width='6'>
                <h2>Ingredient Filters</h2>
            </Grid.Column>
        </Grid>
    )
})
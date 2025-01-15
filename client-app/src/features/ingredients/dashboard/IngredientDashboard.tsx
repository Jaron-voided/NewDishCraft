import {Grid, Segment} from "semantic-ui-react";
import IngredientList from "./IngredientList.tsx";

import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";
import {useEffect} from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";
import IngredientFilters from "./IngredientFilters.tsx";


export default observer(function IngredientDashboard() {

    const {ingredientStore} = useStore();
    const {loadIngredients, ingredientRegistry} = ingredientStore;

    useEffect(() => {
        if (ingredientRegistry.size <= 1) loadIngredients();
/*        if (ingredientStore.ingredientsByPrice.length === 0) {
            loadIngredients();
        }*/
    }, [loadIngredients, ingredientRegistry.size]);


    if (ingredientStore.loadingInitial) return <LoadingComponent content='Loading ingredients...' />

    return (
        <Grid centered>
            <Grid.Column width={14}>
                <Segment textAlign="center">
                    <IngredientList />
                </Segment>
            </Grid.Column>
        </Grid>
    );

/*
    return (
        <Segment clearing textAlign="center" style={{ width: "80%", margin: "0 auto" }}>
            <IngredientList />
        </Segment>
    );
*/

    /*    return (
            <Grid>
               <Grid.Column center width='14'>
                    <IngredientList />
               </Grid.Column>
    {/!*            <Grid.Column width='6'>
                    <IngredientFilters />
                </Grid.Column>*!/}
            </Grid>
        )*/
})
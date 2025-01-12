import {Grid} from "semantic-ui-react";
import {useStore} from "../../../app/stores/store.ts";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";
import {observer} from "mobx-react-lite";
import {useParams} from "react-router-dom";
import {useEffect} from "react";
import IngredientDetailedChat from "./IngredientDetailedChat.tsx";
import IngredientDetailedInfo from "./IngredientDetailedInfo.tsx";
import IngredientDetailedHeader from "./IngredientDetailedHeader.tsx";
import IngredientDetailedSidebar from "./IngredientDetailedSidebar.tsx";



export default observer(function IngredientDetails() {
    const {ingredientStore} = useStore();
    const {selectedIngredient: ingredient, loadIngredient, loadingInitial} = ingredientStore;
    const {id} = useParams();

    useEffect(() => {
        if (id) loadIngredient(id);
    }, [id, loadIngredient]);

    if (loadingInitial || !ingredient) return <LoadingComponent />;

    return (
        <Grid>
            <Grid.Column width={10}>
                <IngredientDetailedHeader ingredient={ingredient} />
                <IngredientDetailedInfo ingredient={ingredient}/>
               {/* <IngredientDetailedChat />*/}
            </Grid.Column>
            <Grid.Column width={6}>
        {/*        <IngredientDetailedSidebar />*/}
            </Grid.Column>
        </Grid>
    )
})
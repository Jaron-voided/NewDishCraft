import {Grid} from "semantic-ui-react";
import IngredientList from "./IngredientList.tsx";
import IngredientDetails from "../details/IngredientDetails.tsx";
import IngredientForm from "../form/IngredientForm.tsx";
import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";


export default observer(function IngredientDashboard() {

    const {ingredientStore} = useStore();
    const {selectedIngredient, editMode} = ingredientStore;

    return (
        <Grid>
           <Grid.Column width='10'>
                <IngredientList />
           </Grid.Column>
            <Grid.Column width='6'>
                {selectedIngredient && ! editMode && /*only loads if something exists*/
                <IngredientDetails />}
                {editMode &&
                <IngredientForm />}
            </Grid.Column>
        </Grid>
    )
})
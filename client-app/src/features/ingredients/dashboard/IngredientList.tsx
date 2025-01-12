import {Header} from "semantic-ui-react";
import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";
import IngredientListItem from "./IngredientListItem.tsx";
import {Fragment} from "react";


export default observer(function IngredientList() {
    const {ingredientStore} = useStore();
    const {groupedIngredients} = ingredientStore;

    return (
        <>
            {groupedIngredients.map(([category, ingredients]) => (
                <Fragment key={category}>
                    <Header sub color='teal'>
                        {category}
                    </Header>
                    {ingredients.map(ingredient => (
                        <IngredientListItem key={ingredient.id} ingredient={ingredient}/>
                    ))}
                </Fragment>
            ))}
        </>
    );
})
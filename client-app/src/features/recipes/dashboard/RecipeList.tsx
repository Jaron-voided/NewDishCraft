import {Header} from "semantic-ui-react";
import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";
import RecipeListItem from "./RecipeListItem.tsx";
import {Fragment} from "react";


export default observer(function RecipeList() {
    const {recipeStore} = useStore();
    const {groupedRecipes} = recipeStore;

    return (
        <>
            {groupedRecipes.map(([category, recipes]) => (
                <Fragment key={category}>
                    <Header sub color='teal'>
                        {category}
                    </Header>
                    {recipes.map(recipe => (
                        <RecipeListItem key={recipe.id} recipe={recipe}/>
                    ))}
                </Fragment>
            ))}
        </>

    );
})
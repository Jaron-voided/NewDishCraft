import { observer } from "mobx-react-lite";
import { useStore } from "../../app/stores/store.ts";
import { Container, Header, Segment } from "semantic-ui-react";

export default observer(function UserProfile() {
    const { userStore, /*ingredientStore, recipeStore*/ } = useStore();
    const { user } = userStore;

/*
    const totalIngredients = Array.from(ingredientStore.ingredientRegistry.values())
        .filter(ingredient => ingredient.appUserId === user?.username).length;

    const totalRecipes = Array.from(recipeStore.recipeRegistry?.values() || [])
        .filter(recipe => recipe.appUserId === user?.username).length;
*/

    return (
        <Segment>
            <Container textAlign="center">
                <Header as="h1" style={{ marginBottom: "1em" }}>
                    Welcome, {user?.displayName}!
                </Header>
                <p>
                    YOOO
{/*
                    You have <strong>{totalIngredients}</strong> ingredients and <strong>{totalRecipes}</strong> recipes.
*/}
                </p>
            </Container>
        </Segment>
    );
});

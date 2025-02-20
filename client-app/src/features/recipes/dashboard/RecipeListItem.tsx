import { Button, Icon, Item, Segment } from "semantic-ui-react";
import {Link, useNavigate} from "react-router-dom";
import { Recipe } from "../../../app/models/recipe.ts";
import { useStore } from "../../../app/stores/store.ts";
import styles from "./RecipeListItem.module.css";
import { observer } from "mobx-react-lite";

interface Props {
    recipe: Recipe;
}

export default observer(function RecipeListItem({ recipe }: Props) {
    const { recipeStore } = useStore();
    const { deleteRecipe, loading } = recipeStore;
    const navigate = useNavigate();

    const handleDelete = async () => {
        const deleted = await deleteRecipe(recipe.id);
        if (deleted) {
            navigate('/recipes');
        }
    };
    return (
        <Segment.Group className={styles.card}>
            {/* Top row: Image, Name, Edit, and View buttons */}
            <Segment className={styles.topSegment}>
                <Item.Group>
                    <Item>
                        <Item.Image
                            src={`/assets/categoryImages/${recipe.recipeCategory}.jpg`}
                            className={styles.image}
                        />
                        <Item.Content>
                            <Item.Header className={styles.header}>
                                {recipe.name}
                            </Item.Header>
                            <Item.Description>{recipe.recipeCategory}</Item.Description>
                        </Item.Content>
                    </Item>
                </Item.Group>
                <div className={styles.buttons}>
                    <Button
                        as={Link}
                        to={`/recipes/manage/${recipe.id}`}
                        color="teal"
                        content="Edit"
                        className={styles.editButton}
                    />
                    <Button
                        as={Link}
                        to={`/recipes/${recipe.id}`}
                        color="blue"
                        content="View"
                        className={styles.viewButton}
                    />
                    <Button
                        color="red"
                        content="Delete"
                        onClick={handleDelete}
                        loading={loading}
                        className={styles.deleteButton}
                    />
                </div>
            </Segment>

            {/* Recipe Information */}
            <Segment secondary className={styles.recipeInfoSegment}>
                <div className={styles.recipeInfoGrid}>
                    <span>
                        <Icon name="users" />
                        Servings: {recipe.servingsPerRecipe}
                    </span>
                    <span>
                        <Icon name="dollar" />
                        Total Price: ${recipe.totalPrice.toFixed(2)}
                    </span>
                    <span>
                        <Icon name="dollar sign" />
                        Price Per Serving: ${recipe.pricePerServing.toFixed(2)}
                    </span>
                </div>
            </Segment>

            {/* Nutritional Information */}
            <Segment secondary className={styles.nutritionSegment}>
                <div className={styles.nutritionGrid}>
                    <span>
                        <Icon name="food" />
                        Calories: {recipe.nutrition?.totalCalories ?? recipe.caloriesPerRecipe}
                    </span>
                    <span>
                        <Icon name="leaf" />
                        Carbs: {recipe.nutrition?.totalCarbs ?? recipe.carbsPerRecipe}g
                    </span>
                    <span>
                        <Icon name="fire" />
                        Fat: {recipe.nutrition?.totalFat ?? recipe.fatPerRecipe}g
                    </span>
                    <span>
                        <Icon name="utensil spoon" />
                        Protein: {recipe.nutrition?.totalProtein ?? recipe.proteinPerRecipe}g
                    </span>
                </div>
            </Segment>
        </Segment.Group>
    );
});


/*
import { Button, Item, Segment } from "semantic-ui-react";
import { Link } from "react-router-dom";
import { Recipe } from "../../../app/models/recipe.ts";
import { useStore } from "../../../app/stores/store.ts";
import styles from "./RecipeListItem.module.css";
import {observer} from "mobx-react-lite";

interface Props {
    recipe: Recipe;
}

export default observer(function RecipeListItem({ recipe }: Props) {
    const { recipeStore } = useStore();
    const { deleteRecipe, loading } = recipeStore;

    const handleDelete = () => {
        deleteRecipe(recipe.id);
    };

    return (
        <Segment.Group className={styles.card}>
            {/!* Top Row: Image, Name, Edit, and View buttons *!/}
            <Segment className={styles.topSegment}>
                <Item.Group>
                    <Item>
                        <Item.Image
                            src={`/assets/categoryImages/${recipe.recipeCategory}.jpg`}
                            className={styles.image}
                        />
                        <Item.Content>
                            <Item.Header className={styles.header}>
                                {recipe.name}, {recipe.totalPrice} , {recipe.caloriesPerRecipe}, {recipe.pricePerServing}
                            </Item.Header>
                        </Item.Content>
                    </Item>
                </Item.Group>
                <div className={styles.topButtons}>
                    <Button
                        as={Link}
                        to={`/recipes/manage/${recipe.id}`}
                        color="google plus"
                        content="Edit"
                        className={styles.editButton}
                    />
                    <Button
                        as={Link}
                        to={`/recipes/${recipe.id}`}
                        color="teal"
                        content="View"
                        className={styles.viewButton}
                    />
                </div>
            </Segment>

            {/!* Placeholder for future content *!/}
            <Segment secondary className={styles.placeholderSegment}>
                <span>Something else here...</span>
            </Segment>

            {/!* Bottom Row: Delete Button *!/}
            <Segment className={styles.bottomSegment}>
                <Button
                    color="red"
                    content="Delete"
                    onClick={handleDelete}
                    loading={loading}
                    className={styles.deleteButton}
                />
            </Segment>
        </Segment.Group>
    );
})
*/

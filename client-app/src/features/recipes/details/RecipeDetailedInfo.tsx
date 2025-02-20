import { observer } from 'mobx-react-lite';
import { Segment, Icon } from 'semantic-ui-react';
import { Recipe } from "../../../app/models/recipe";
import styles from './RecipeDetailedInfo.module.css';

interface Props {
    recipe: Recipe
}

export default observer(function RecipeDetailedInfo({recipe}: Props) {
    return (
        <Segment.Group>
            <Segment className={styles.infoSegment}>
                <div className={styles.infoGrid}>
                    <span>
                        <Icon name='users' />
                        Servings: {recipe.servingsPerRecipe}
                    </span>
                    <span>
                        <Icon name='dollar' />
                        Total Price: ${recipe.totalPrice.toFixed(2)}
                    </span>
                    <span>
                        <Icon name='dollar sign' />
                        Price Per Serving: ${recipe.pricePerServing.toFixed(2)}
                    </span>
                </div>
            </Segment>

            <Segment className={styles.nutritionSegment}>
                <div className={styles.nutritionGrid}>
                    <span>
                        <Icon name='food' />
                        Calories: {recipe.nutrition
                        ? Math.round(recipe.nutrition.caloriesPerServing)
                        : Math.round(recipe.caloriesPerRecipe / recipe.servingsPerRecipe)}
                    </span>
                    <span>
                        <Icon name='leaf' />
                        Carbs: {recipe.nutrition
                        ? Math.round(recipe.nutrition.carbsPerServing)
                        : Math.round(recipe.carbsPerRecipe / recipe.servingsPerRecipe)}g
                    </span>
                    <span>
                        <Icon name='fire' />
                        Fat: {recipe.nutrition
                        ? Math.round(recipe.nutrition.fatPerServing)
                        : Math.round(recipe.fatPerRecipe / recipe.servingsPerRecipe)}g
                    </span>
                    <span>
                        <Icon name='utensil spoon' />
                        Protein: {recipe.nutrition
                        ? Math.round(recipe.nutrition.proteinPerServing)
                        : Math.round(recipe.proteinPerRecipe / recipe.servingsPerRecipe)}g
                    </span>
                </div>
            </Segment>

            <Segment className={styles.instructionsSegment}>
                <h3>Instructions:</h3>
                <ol className={styles.instructionsList}>
                    {recipe.instructions.map((instruction, index) => (
                        <li key={index}>{instruction}</li>
                    ))}
                </ol>
            </Segment>
        </Segment.Group>
    );
});

/*import { observer } from 'mobx-react-lite';
import { Segment, Icon } from 'semantic-ui-react';
import { Recipe } from "../../../app/models/recipe";
import styles from './RecipeDetailedInfo.module.css';

interface Props {
    recipe: Recipe
}

export default observer(function RecipeDetailedInfo({recipe}: Props) {
    const caloriesPerServing = Math.round(recipe.caloriesPerRecipe / recipe.servingsPerRecipe);
    const carbsPerServing = Math.round(recipe.carbsPerRecipe / recipe.servingsPerRecipe);
    const fatPerServing = Math.round(recipe.fatPerRecipe / recipe.servingsPerRecipe);
    const proteinPerServing = Math.round(recipe.proteinPerRecipe / recipe.servingsPerRecipe);

    return (
        <Segment.Group>
            <Segment className={styles.infoSegment}>
                <div className={styles.infoGrid}>
                    <span>
                        <Icon name='users' />
                        Servings: {recipe.servingsPerRecipe}
                    </span>
                    <span>
                        <Icon name='dollar' />
                        Total Price: ${recipe.totalPrice.toFixed(2)}
                    </span>
                    <span>
                        <Icon name='dollar sign' />
                        Price Per Serving: ${recipe.pricePerServing.toFixed(2)}
                    </span>
                </div>
            </Segment>

            <Segment className={styles.nutritionSegment}>
                <div className={styles.nutritionGrid}>
                    <span>
                        <Icon name='food' />
                        Calories: {caloriesPerServing}
                    </span>
                    <span>
                        <Icon name='leaf' />
                        Carbs: {carbsPerServing}
                    </span>
                    <span>
                        <Icon name='fire' />
                        Fat: {fatPerServing}
                    </span>
                    <span>
                        <Icon name='utensil spoon' />
                        Protein: {proteinPerServing}
                    </span>
                </div>
            </Segment>

            <Segment className={styles.instructionsSegment}>
                <h3>Instructions:</h3>
                <ol className={styles.instructionsList}>
                    {recipe.instructions.map((instruction, index) => (
                        <li key={index}>{instruction}</li>
                    ))}
                </ol>
            </Segment>
        </Segment.Group>
    );
});*/

/*
import { observer } from 'mobx-react-lite';
import {Segment, Grid, Icon} from 'semantic-ui-react'
import {Recipe} from "../../../app/models/recipe";

interface Props {
    recipe: Recipe
}

export default observer(function RecipeDetailedInfo({recipe}: Props) {
    return (
        <Segment.Group>
            <Segment attached='top'>
                <Grid>
                    <Grid.Column width={1}>
                        <Icon size='large' color='teal' name='info'/>
                    </Grid.Column>
                    <Grid.Column width={15}>
                        <p>{recipe.name}</p>
                    </Grid.Column>
                </Grid>
            </Segment>
            <Segment attached>
                <Grid verticalAlign='middle'>
                    <Grid.Column width={1}>
                        <Icon name='calendar' size='large' color='teal'/>
                    </Grid.Column>
                    <Grid.Column width={15}>
            <span>
              {recipe.servingsPerRecipe}
            </span>
                    </Grid.Column>
                </Grid>
            </Segment>
            <Segment attached>
                <Grid verticalAlign='middle'>
                    <Grid.Column width={1}>
                        <Icon name='marker' size='large' color='teal'/>
                    </Grid.Column>
                    <Grid.Column width={11}>
                        <span>{recipe.name}, {recipe.name}</span>
                    </Grid.Column>
                </Grid>
            </Segment>
        </Segment.Group>
    )
})

*/

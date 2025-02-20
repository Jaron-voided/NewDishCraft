import { observer } from 'mobx-react-lite';
import { Button, Header, Segment, Image } from 'semantic-ui-react';
import { Recipe } from "../../../app/models/recipe";
import {Link, useNavigate} from 'react-router-dom';
import { useStore } from '../../../app/stores/store';
import styles from './RecipeDetailedHeader.module.css';

interface Props {
    recipe: Recipe
}

export default observer(function RecipeDetailedHeader({ recipe }: Props) {
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
        <Segment.Group>
            <Segment className={styles.imageSegment}>
                <Image src={`/assets/categoryImages/${recipe.recipeCategory}.jpg`} className={styles.image} />
                <div className={styles.overlay}>
                    <Header as='h1' content={recipe.name} className={styles.recipeName} />
                    <p className={styles.recipeCategory}>{recipe.recipeCategory}</p>
                </div>
            </Segment>
            <Segment clearing className={styles.buttonSegment}>
                <Button
                    as={Link}
                    to={`/recipes/manage/${recipe.id}`}
                    color='teal'
                    content='Edit'
                    floated='left'
                />
                <Button
                    color='red'
                    content='Delete'
                    floated='right'
                    onClick={handleDelete}
                    loading={loading}
                />
            </Segment>
        </Segment.Group>
    );
});

/*
import { observer } from 'mobx-react-lite';
import {Button, Header, Item, Segment, Image} from 'semantic-ui-react'
import {Recipe} from "../../../app/models/recipe";

const recipeImageStyle = {
    filter: 'brightness(30%)'
};

const recipeImageTextStyle = {
    position: 'absolute',
    bottom: '5%',
    left: '5%',
    width: '100%',
    height: 'auto',
    color: 'white'
};

interface Props {
    recipe: Recipe
}

export default observer (function RecipeDetailedHeader({recipe}: Props) {
    return (
        <Segment.Group>
            <Segment basic attached='top' style={{padding: '0'}}>
                <Image src={`/assets/categoryImages/${recipe.recipeCategory}.jpg`} fluid style={recipeImageStyle}/>
                <Segment style={recipeImageTextStyle} basic>
                    <Item.Group>
                        <Item>
                            <Item.Content>
                                <Header
                                    size='huge'
                                    content={recipe.name}
                                    style={{color: 'white'}}
                                />
                                <p>{recipe.servingsPerRecipe}</p>
                                <p>
                                    Hosted by <strong>Bob</strong>
                                </p>
                            </Item.Content>
                        </Item>
                    </Item.Group>
                </Segment>
            </Segment>
            <Segment clearing attached='bottom'>
                <Button color='teal'>Join Recipe</Button>
                <Button>Cancel attendance</Button>
                <Button color='orange' floated='right'>
                    Manage Event
                </Button>
            </Segment>
        </Segment.Group>
    )
})


*/

import {Button, Icon, Item, Segment} from "semantic-ui-react";
import {Link} from "react-router-dom";
import {Recipe} from "../../../app/models/recipe.ts";
//import {useStore} from "../../../app/stores/store.ts";

interface Props {
    recipe: Recipe;
}


export default function RecipeListItem({recipe}: Props) {

    //const {recipeStore} = useStore();


    return (

        <Segment.Group>
            <Segment>
                <Item.Group>
                    <Item>
                        <Item.Image size='tiny' circular src={"/assets/logo.png"} />
                        <Item.Content>
                            <Item.Header as={Link} to={`/recipes/${recipe.id}`}>
                                {recipe.name}
                            </Item.Header>
                            <Item.Description>Stuff to go here</Item.Description>
                        </Item.Content>
                    </Item>
                </Item.Group>
            </Segment>
            <Segment>
                <span>
                    <Icon name='clock' /> {recipe.recipeCategory}
                    <Icon name='dollar' /> {recipe.servingsPerRecipe}
                    <Icon name='marker' /> {recipe.name}
                </span>
            </Segment>
            <Segment secondary>
                Something else here...
            </Segment>
            <Segment clearing>
                <span>{recipe.recipeCategory}</span>
                <Button
                    as={Link}
                    to={`/recipes/${recipe.id}`}
                    color='teal'
                    floated='right'
                    content='View'
                />
            </Segment>
        </Segment.Group>
    )
}
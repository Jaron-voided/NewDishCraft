import {Button, Icon, Item, Segment} from "semantic-ui-react";
import {Link} from "react-router-dom";
import {Ingredient} from "../../../app/models/ingredient.ts";
//import {useStore} from "../../../app/stores/store.ts";

interface Props {
    ingredient: Ingredient;
}


export default function IngredientListItem({ingredient}: Props) {

    //const {ingredientStore} = useStore();
/*
    const {deleteIngredient, loading} = ingredientStore;

    const [target, setTarget] = useState('');

    function handleIngredientDelete(e: SyntheticEvent<HTMLButtonElement>, id: string) {
        setTarget(e.currentTarget.name);
        deleteIngredient(id);
    }
*/


    return (

        <Segment.Group>
            <Segment>
                <Item.Group>
                    <Item>
                        <Item.Image size='tiny' circular src={"/assets/logo.png"} />
                        <Item.Content>
                            <Item.Header as={Link} to={`/ingredients/${ingredient.id}`}>
                                {ingredient.name}
                            </Item.Header>
                            <Item.Description>Stuff to go here</Item.Description>
                        </Item.Content>
                    </Item>
                </Item.Group>
            </Segment>
            <Segment>
                <span>
                    <Icon name='clock' /> {ingredient.category}
                    <Icon name='dollar' /> {ingredient.pricePerPackage}
                    <Icon name='marker' /> {ingredient.measurementsPerPackage}
                </span>
            </Segment>
            <Segment secondary>
                Something else here...
            </Segment>
            <Segment clearing>
                <span>{ingredient.category}</span>
                <Button
                    as={Link}
                    to={`/ingredients/${ingredient.id}`}
                    color='teal'
                    floated='right'
                    content='View'
                />
            </Segment>
        </Segment.Group>
 /*       <Item key={ingredient.id} style={{marginBottom: "2em", padding: "1em", border: "1px solid #ccc", borderRadius: "8px"}}>
            <Item.Content>
                {/!* Header Section *!/}
                <div style={{display: "flex", justifyContent: "space-between", alignItems: "center"}}>
                    <Header as="h2" style={{marginBottom: "0.2em"}}>
                        {ingredient.name}
                    </Header>
                    <Button
                        as={Link} to={`/ingredients/${ingredient.id}`}
                        floated='right'
                        color="blue"
                        content="View"
                    />
                    <Button
                        onClick={(e) => handleIngredientDelete(e, ingredient.id)}
                        name={ingredient.id}
                        loading={loading && target === ingredient.id}
                        floated='right'
                        color="red"
                        content="Delete"
                    />
                </div>
                <Item.Meta style={{color: "teal", fontSize: "1em", marginBottom: "1em"}}>
                    {ingredient.category}
                </Item.Meta>

                {/!* Description Section *!/}

            </Item.Content>
        </Item>*/
    )
}
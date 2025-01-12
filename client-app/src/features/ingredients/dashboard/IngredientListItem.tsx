import {Button, Icon, Item, Segment} from "semantic-ui-react";
import {Link} from "react-router-dom";
import {Ingredient} from "../../../app/models/ingredient.ts";
import {useStore} from "../../../app/stores/store.ts";

interface Props {
    ingredient: Ingredient;
}


export default function IngredientListItem({ingredient}: Props) {
    const {ingredientStore} = useStore();
    const {deleteIngredient, loading} = ingredientStore;

    const handleDelete = () => {
        deleteIngredient(ingredient.id);
    }

    return (
        <Segment.Group>
            <Segment>
                <Item.Group>
                    <Item>
                        <Item.Image size='tiny' circular src={`/assets/categoryImages/${ingredient.category}.jpg`} />
                        <Item.Content textAlign='center'>
                            <Item.Header
                                as={Link}
                                to={`/ingredients/${ingredient.id}`}
                                className='ui center aligned'
                                style={{ color: 'teal' }}

                            >
                                {ingredient.name}
                            </Item.Header>
                            <Item.Description
                                className='ui center aligned'
                                style={{ color: 'salmon' }}
                            >
                                {ingredient.category} {ingredient.measurementUnit?.measuredIn}
                            </Item.Description>
                        </Item.Content>
                    </Item>
                </Item.Group>
            </Segment>
            <Segment textAlign="center" style={{ color: "salmon" }}>
                <span style={{ display: "block", marginBottom: "5px" }}>
                    <Icon name="balance scale" />
                    Measurements Per Package: {ingredient.measurementsPerPackage}
                </span>
                            <span style={{ display: "block", marginBottom: "5px" }}>
                    <Icon name="dollar" />
                    Price Per Package: {ingredient.pricePerPackage}
                </span>
                <span style={{ display: "block" }}>
                    <Icon name="dollar sign" />
                    Price Per Measurement: {(ingredient.pricePerPackage / ingredient.measurementsPerPackage).toFixed(2)}
                </span>
            </Segment>

            <Segment secondary textAlign="center">
                <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "10px" }}>
                    <span>
                        <Icon name="food" />
                        Calories: {ingredient.nutrition?.calories || "N/A"}
                    </span>
                                <span>
                        <Icon name="leaf" />
                        Carbs: {ingredient.nutrition?.carbs || "N/A"}
                    </span>
                                <span>
                        <Icon name="fire" />
                        Fat: {ingredient.nutrition?.fat || "N/A"}
                    </span>
                                <span>
                        <Icon name="utensil spoon" />
                        Protein: {ingredient.nutrition?.protein || "N/A"}
                    </span>
                </div>
            </Segment>

            <Segment clearing>
                <span>XYZ recipes use this ingredient!</span>
                <Button
                    as={Link}
                    to={`/ingredients/${ingredient.id}`}
                    color='teal'
                    floated='right'
                    content='View'
                />
                <Button
                    as={Link}
                    to={`/manage/${ingredient.id}`}
                    color='orange'
                    floated='right'
                    content='Edit'
                />
                <Button
                    color='red'
                    floated='right'
                    content='Delete'
                    onClick={handleDelete}
                    loading={loading}
                />


            </Segment>
        </Segment.Group>
    )
}
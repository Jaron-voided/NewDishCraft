import {Segment, Item, Button, Header/*, Divider*/} from "semantic-ui-react";
import {SyntheticEvent, useState} from "react";
import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";


export default observer(function IngredientList() {
    const {ingredientStore} = useStore();
    const {deleteIngredient, ingredientsByPrice, loading} = ingredientStore;

    const [target, setTarget] = useState('');

    function handleIngredientDelete(e: SyntheticEvent<HTMLButtonElement>, id: string) {
        setTarget(e.currentTarget.name);
        deleteIngredient(id);
    }


    return (
        <Segment>
            <Item.Group divided>
                {ingredientsByPrice.map(ingredient => (
                    <Item key={ingredient.id} style={{marginBottom: "2em", padding: "1em", border: "1px solid #ccc", borderRadius: "8px"}}>
                        <Item.Content>
                            {/* Header Section */}
                            <div style={{display: "flex", justifyContent: "space-between", alignItems: "center"}}>
                                <Header as="h2" style={{marginBottom: "0.2em"}}>
                                    {ingredient.name}
                                </Header>
                                <Button
                                    onClick={() => ingredientStore.selectIngredient(ingredient.id)}
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

                            {/* Description Section */}

                        </Item.Content>
                    </Item>
                ))}
            </Item.Group>
        </Segment>
    );
})
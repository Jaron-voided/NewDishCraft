import { Segment, List, Item, Button } from 'semantic-ui-react';
import { observer } from 'mobx-react-lite';

import styles from './RecipeDetailedSidebar.module.css';
import {Measurement} from "../../../app/models/recipe.ts";

interface Props {
    measurements: Measurement[];
}

export default observer(function RecipeDetailedSidebar({ measurements }: Props) {
    return (
        <>
            <Segment
                textAlign='center'
                className={styles.header}
                attached='top'
                secondary
                inverted
                color='teal'
            >
                Ingredients
            </Segment>
            <Segment attached className={styles.measurementsSegment}>
                <Item.Group relaxed divided>
                    {measurements.map((measurement) => (
                        <Item key={measurement.id} className={styles.measurementItem}>
                            <Item.Content>
                                <Item.Header as='h4'>{measurement.ingredientName}</Item.Header>
                                <Item.Description>
                                    Amount: {measurement.amount}
                                </Item.Description>
                                <Item.Extra>
                                    Calories: {measurement.caloriesPerAmount}
                                </Item.Extra>
                            </Item.Content>
                        </Item>
                    ))}
                </Item.Group>
            </Segment>
            <Button
                color='teal'
                fluid
                className={styles.editButton}
            >
                Edit Measurements
            </Button>
        </>
    );
});

/*
import { Segment, List, Label, Item, Image } from 'semantic-ui-react'
import { Link } from 'react-router-dom'
import { observer } from 'mobx-react-lite'

export default observer(function RecipeDetailedSidebar () {
    return (
        <>
            <Segment
                textAlign='center'
                style={{ border: 'none' }}
                attached='top'
                secondary
                inverted
                color='teal'
            >
                Recipes
            </Segment>
            <Segment attached>
                <List relaxed divided>
                    <Item style={{ position: 'relative' }}>
                        <Label
                            style={{ position: 'absolute' }}
                            color='orange'
                            ribbon='right'
                        >
                            Host
                        </Label>
                        <Image size='tiny' src={'/assets/user.png'} />
                        <Item.Content verticalAlign='middle'>
                            <Item.Header as='h3'>
                                <Link to={`#`}>Bob</Link>
                            </Item.Header>
                            <Item.Extra style={{ color: 'orange' }}>Following</Item.Extra>
                        </Item.Content>
                    </Item>

                    <Item style={{ position: 'relative' }}>
                        <Image size='tiny' src={'/assets/user.png'} />
                        <Item.Content verticalAlign='middle'>
                            <Item.Header as='h3'>
                                <Link to={`#`}>Tom</Link>
                            </Item.Header>
                            <Item.Extra style={{ color: 'orange' }}>Following</Item.Extra>
                        </Item.Content>
                    </Item>

                    <Item style={{ position: 'relative' }}>
                        <Image size='tiny' src={'/assets/user.png'} />
                        <Item.Content verticalAlign='middle'>
                            <Item.Header as='h3'>
                                <Link to={`#`}>Sally</Link>
                            </Item.Header>
                        </Item.Content>
                    </Item>
                </List>
            </Segment>
        </>
    )
})



*/

import { observer } from 'mobx-react-lite';
import { Segment, Grid, Icon, List } from 'semantic-ui-react'
import { DayPlan } from "../../../app/models/dayPlan";

interface Props {
    dayPlan: DayPlan
}

export default observer(function DayPlanDetailedInfo({dayPlan}: Props) {
    return (
        <Segment.Group>
            <Segment attached='top'>
                <Grid>
                    <Grid.Column width={1}>
                        <Icon size='large' color='teal' name='book'/>
                    </Grid.Column>
                    <Grid.Column width={15}>
                        <p>Recipes for the Day</p>
                    </Grid.Column>
                </Grid>
            </Segment>
            {dayPlan.dayPlanRecipes.map(dayPlanRecipe => (
                <Segment key={dayPlanRecipe.id} attached>
                    <Grid verticalAlign='middle'>
                        <Grid.Column width={1}>
                            <Icon name='food' size='large' color='teal'/>
                        </Grid.Column>
                        <Grid.Column width={7}>
                            <span>{dayPlanRecipe.recipeName}</span>
                        </Grid.Column>
                        <Grid.Column width={4}>
                            <Icon name='chart bar' />
                            <span>{dayPlanRecipe.servings} servings</span>
                        </Grid.Column>
                        <Grid.Column width={4}>
                            <Icon name='dollar' />
                            <span>${(dayPlanRecipe.pricePerServing * dayPlanRecipe.servings).toFixed(2)}</span>
                        </Grid.Column>
                    </Grid>
                    <Grid>
                        <Grid.Column width={16}>
                            <List horizontal>
                                <List.Item>
                                    <Icon name='fire' />
                                    {dayPlanRecipe.caloriesPerRecipe} cal
                                </List.Item>
                                <List.Item>
                                    <Icon name='leaf' />
                                    {dayPlanRecipe.carbsPerRecipe}g carbs
                                </List.Item>
                                <List.Item>
                                    <Icon name='puzzle piece' />
                                    {dayPlanRecipe.proteinPerRecipe}g protein
                                </List.Item>
                                <List.Item>
                                    <Icon name='tint' />
                                    {dayPlanRecipe.fatPerRecipe}g fat
                                </List.Item>
                            </List>
                        </Grid.Column>
                    </Grid>
                </Segment>
            ))}
        </Segment.Group>
    )
})
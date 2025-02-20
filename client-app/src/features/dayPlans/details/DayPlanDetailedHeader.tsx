import { observer } from 'mobx-react-lite';
import {Button, Header, Item, Segment, Image, Statistic} from 'semantic-ui-react'
import {DayPlan} from "../../../app/models/dayPlan";

const dayPlanImageStyle = {
    filter: 'brightness(30%)'
};

const dayPlanImageTextStyle = {
    position: 'absolute',
    bottom: '5%',
    left: '5%',
    width: '100%',
    height: 'auto',
    color: 'white'
};

interface Props {
    dayPlan: DayPlan
}

export default observer (function DayPlanDetailedHeader({dayPlan}: Props) {
    return (
        <Segment.Group>
            <Segment basic attached='top' style={{padding: '0'}}>
                <Image src={`/assets/logo.png`} fluid style={dayPlanImageStyle}/>
                <Segment style={dayPlanImageTextStyle} basic>
                    <Item.Group>
                        <Item>
                            <Item.Content>
                                <Header
                                    size='huge'
                                    content={dayPlan.name}
                                    style={{color: 'white'}}
                                />
                                <p>
                                    {dayPlan.dayPlanRecipes.length} Recipes | Total Cost: ${dayPlan.priceForDay.toFixed(2)}
                                </p>
                            </Item.Content>
                        </Item>
                    </Item.Group>
                </Segment>
            </Segment>
            <Segment attached>
                <Statistic.Group widths='four'>
                    <Statistic>
                        <Statistic.Value>{dayPlan.caloriesPerDay}</Statistic.Value>
                        <Statistic.Label>Calories</Statistic.Label>
                    </Statistic>
                    <Statistic>
                        <Statistic.Value>{dayPlan.proteinPerDay}g</Statistic.Value>
                        <Statistic.Label>Protein</Statistic.Label>
                    </Statistic>
                    <Statistic>
                        <Statistic.Value>{dayPlan.carbsPerDay}g</Statistic.Value>
                        <Statistic.Label>Carbs</Statistic.Label>
                    </Statistic>
                    <Statistic>
                        <Statistic.Value>{dayPlan.fatPerDay}g</Statistic.Value>
                        <Statistic.Label>Fat</Statistic.Label>
                    </Statistic>
                </Statistic.Group>
            </Segment>
            <Segment clearing attached='bottom'>
                <Button
                    color='blue'
                    floated='right'
                    content='Edit Day Plan'
                />
                <Button
                    color='red'
                    floated='right'
                    content='Delete Day Plan'
                />
            </Segment>
        </Segment.Group>
    )
})
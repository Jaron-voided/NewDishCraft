import { Button, Icon, Item, Segment } from "semantic-ui-react";
import { Link } from "react-router-dom";
import { DayPlan } from "../../../app/models/dayPlan";
import { useStore } from "../../../app/stores/store";
import styles from "./DayPlanListItem.module.css";

interface Props {
    dayPlan: DayPlan;
}

export default function DayPlanListItem({ dayPlan }: Props) {
    const { dayPlanStore } = useStore();
    const { deleteDayPlan, loading } = dayPlanStore;

    const handleDelete = () => {
        deleteDayPlan(dayPlan.id);
    };

    return (
        <Segment.Group className={styles.card}>
            {/* Top Row: Image, Name, Edit, and View buttons */}
            <Segment className={styles.topSegment}>
                <Item.Group>
                    <Item>
                        <Item.Image
                            src={`/assets/logo.png`}
                            className={styles.image}
                        />
                        <Item.Content>
                            <Item.Header className={styles.header}>
                                {dayPlan.name}
                            </Item.Header>
                        </Item.Content>
                    </Item>
                </Item.Group>
                <div className={styles.topButtons}>
                    <Button
                        as={Link}
                        to={`/dayPlan/manage/${dayPlan.id}`}
                        color="google plus"
                        content="Edit"
                        className={styles.editButton}
                    />
                    <Button
                        as={Link}
                        to={`/dayPlan/${dayPlan.id}`}
                        color="teal"
                        content="View"
                        className={styles.viewButton}
                    />
                </div>
            </Segment>

            {/* Nutritional Information */}
            <Segment secondary className={styles.nutritionSegment}>
                <div className={styles.nutritionGrid}>
                    <span>
                        <Icon name="food" />
                        Calories: {dayPlan.caloriesPerDay}
                    </span>
                    <span>
                        <Icon name="leaf" />
                        Carbs: {dayPlan.carbsPerDay}g
                    </span>
                    <span>
                        <Icon name="fire" />
                        Fat: {dayPlan.fatPerDay}g
                    </span>
                    <span>
                        <Icon name="utensil spoon" />
                        Protein: {dayPlan.proteinPerDay}g
                    </span>
                </div>
            </Segment>

            {/* Recipe Count and Price */}
            <Segment secondary className={styles.infoSegment}>
                <div className={styles.infoGrid}>
                    <span>
                        <Icon name="book" />
                        Recipes: {dayPlan.dayPlanRecipes.length}
                    </span>
                    <span>
                        <Icon name="dollar" />
                        Total Cost: ${dayPlan.priceForDay.toFixed(2)}
                    </span>
                </div>
            </Segment>

            {/* Bottom Row: Delete Button */}
            <Segment clearing className={styles.bottomSegment}>
                <Button
                    color="red"
                    content="Delete"
                    onClick={handleDelete}
                    loading={loading}
                    className={styles.deleteButton}
                    floated="right"
                />
            </Segment>
        </Segment.Group>
    );
}
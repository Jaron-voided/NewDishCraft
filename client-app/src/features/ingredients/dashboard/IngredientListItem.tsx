import { Button, Icon, Item, Segment } from "semantic-ui-react";
import {Link, useNavigate} from "react-router-dom";
import {Ingredient, MeasuredIn} from "../../../app/models/ingredient.ts";
import { useStore } from "../../../app/stores/store.ts";
import styles from "./IngredientListItem.module.css";
import {observer} from "mobx-react-lite";
import {useEffect, useState} from "react";

interface Props {
    ingredient: Ingredient;
}

export default observer(function IngredientListItem({ ingredient }: Props) {
    const { ingredientStore, recipeStore } = useStore();
    const { deleteIngredientWithConfirmation, loading } = ingredientStore;
    const navigate = useNavigate();
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        console.log('Ingredient Store:', ingredientStore);
        console.log('Ingredients Loaded:', ingredientStore.ingredientsLoaded);
        console.log('Ingredient Registry:', Array.from(ingredientStore.ingredientRegistry.values()));

        const loadData = async () => {
            try {
                // Try to load recipes, but don't fail if there are no recipes
                await recipeStore.loadRecipes().catch(() => {
                    // If recipe loading fails, just log it and continue
                    console.log('No recipes to load');
                });

                // Load ingredients
                await ingredientStore.loadIngredients();
                console.log('Ingredients after loading:', Array.from(ingredientStore.ingredientRegistry.values()));
            } catch (error) {
                console.error('Error loading data', error);
            } finally {
                // Set loading to false once both (attempted) loads are complete
                setIsLoading(false);
            }
        };

        loadData();
    }, [recipeStore, ingredientStore]);

    // New useEffect for logging ingredient details
    useEffect(() => {
        console.group('Ingredient List Item Rendering');
        console.log('Full Ingredient Object:', ingredient);
        console.log('Nutrition Object:', ingredient.nutrition);
        console.log('Nutrition Details:', {
            calories: ingredient.nutrition?.calories,
            protein: ingredient.nutrition?.protein,
            carbs: ingredient.nutrition?.carbs,
            fat: ingredient.nutrition?.fat
        });
        console.groupEnd();
    }, [ingredient]);

    if (isLoading) {
        return <div>Loading...</div>;
    }
    /*
    useEffect(() => {
        if (!recipeStore.recipesLoaded) recipeStore.loadRecipes();
        if (!ingredientStore.ingredientsLoaded) ingredientStore.loadIngredients();
    }, [recipeStore, ingredientStore]);

    if (!recipeStore.recipesLoaded || !ingredientStore.ingredientsLoaded) {
        return <div>Loading...</div>;
    }*/

    const handleDelete = async () => {
        const deleted = await deleteIngredientWithConfirmation(ingredient.id);
        if (deleted) {
            navigate('/ingredients');
        }
    };

    const getMeasurementUnitDisplay = (measurementUnit: Ingredient['measurementUnit']) => {
        if (measurementUnit?.measuredIn === MeasuredIn.Each) {
            return 'Each';
        } else if (measurementUnit?.measuredIn === MeasuredIn.Weight) {
            return measurementUnit?.weightUnit;
        } else if (measurementUnit?.measuredIn === MeasuredIn.Volume) {
            return measurementUnit?.volumeUnit;
        }
        return '';
    };


    const recipeCount = recipeStore.getRecipesUsingIngredient(ingredient.id).length;

    return (
        <Segment.Group className={styles.card}>
            {/* Top row: Image, Name, Edit, and View buttons */}
            <Segment className={styles.topSegment}>
                <Item.Group>
                    <Item>
                        <Item.Image
                            src={`/assets/categoryImages/${ingredient.category}.jpg`}
                            className={styles.image}
                        />
                        <Item.Content>
                            <Item.Header
                                //as={Link}
                                //to={`/ingredients/${ingredient.id}`}
                                className={styles.header}
                            >
                                {ingredient.name}
                            </Item.Header>
                        </Item.Content>
                    </Item>
                </Item.Group>
                <div className={styles.buttons}>
                    <Button
                        as={Link}
                        to={`/ingredients/manage/${ingredient.id}`}
                        color={"teal"}
                        content="Edit"
                        className={styles.editButton}
                    />
                    <Button
                        color="red"
                        content="Delete"
                        onClick={handleDelete}
                        loading={loading}
                        className={styles.deleteButton}
                    />
{/*                    <Button
                        as={Link}
                        to={`/ingredients/${ingredient.id}`}
                        color="teal"
                        content="View"
                        className={styles.viewButton}
                    />*/}
                </div>
            </Segment>

            {/* Nutritional Information */}
            <Segment secondary className={styles.nutritionSegment}>
                <div className={styles.nutritionGrid}>
                    <span>
                        <Icon name="food" />
                        Calories: {ingredient.nutrition?.calories ?? "N/A"}
                    </span>
                    <span>
                        <Icon name="leaf" />
                        Carbs: {ingredient.nutrition?.carbs ?? "N/A"}
                    </span>
                    <span>
                        <Icon name="fire" />
                        Fat: {ingredient.nutrition?.fat ?? "N/A"}
                    </span>
                    <span>
                        <Icon name="utensil spoon" />
                        Protein: {ingredient.nutrition?.protein ?? "N/A"}
                    </span>
                </div>
            </Segment>

            {/* Measurements, Pricing, and Actions */}
            <Segment secondary className={styles.pricingSegment}>
                <div className={styles.pricingInfo}>
                    <div>
                        <Icon name="balance scale" />
                        Measurements Per Package: {`${ingredient.measurementsPerPackage} unit ${getMeasurementUnitDisplay(ingredient.measurementUnit)}`}

                    </div>
                    <div>
                        <Icon name="dollar" />
                        Price Per Package: {ingredient.pricePerPackage}
                    </div>
                    <div>
                        <Icon name="dollar sign" />
                        Price Per Measurement:{" "}
                        {ingredient.pricePerMeasurement}
{/*
                        {(ingredient.pricePerPackage / ingredient.measurementsPerPackage).toFixed(2)}
*/}
                    </div>
                </div>
                <div className={styles.actions}>
                    <span className={styles.recipeInfo}>
                      {recipeCount === 0
                          ? 'No recipes uses this ingredient'
                          : `${recipeCount} recipes use this ingredient`}
                    </span>
{/*                    <Button
                        color="red"
                        content="Delete"
                        onClick={handleDelete}
                        loading={loading}
                        className={styles.deleteButton}
                    />*/}
                </div>
            </Segment>
        </Segment.Group>
    );
})

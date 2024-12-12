import {Card, Image, CardContent, CardDescription, CardHeader, CardMeta, Button} from "semantic-ui-react";
import {useStore} from "../../../app/stores/store.ts";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";



export default function IngredientDetails() {
    const {ingredientStore} = useStore();
    const {selectedIngredient: ingredient, openForm, cancelSelectedIngredient} = ingredientStore;

    if (!ingredient) return <LoadingComponent />;

    return (
        <Card fluid>
            <Image src={`/assets/categoryImages/${ingredient.category}.jpg`}/>
            <CardContent>
                <CardHeader>{ingredient.name}</CardHeader>
                <CardMeta>
                    <span>Total Price: {ingredient.pricePerPackage} </span>
                </CardMeta>
                <CardDescription>
                    {/* Package Cost and Measurements Per Package */}
                    <div style={{ marginBottom: "1em" }}>
                        <strong>Package Cost:</strong> {ingredient.pricePerPackage}
                    </div>
                    <div style={{ marginBottom: "1em" }}>
                        <strong>Measurements Per Package:</strong> {ingredient.measurementsPerPackage}
                    </div>

                    {/* Nutritional Values */}
                    {ingredient.nutrition && (
                        <div style={{ marginBottom: "1em" }}>
                            <strong>Nutritional Values:</strong>
                            <ul>
                                <li>Calories: {ingredient.nutrition.calories}</li>
                                <li>Protein: {ingredient.nutrition.protein}g</li>
                                <li>Carbohydrates: {ingredient.nutrition.carbs}g</li>
                                <li>Fat: {ingredient.nutrition.fat}g</li>
                            </ul>
                        </div>
                    )}

                    {/* Measurements */}
                    {ingredient.measurementUnit && (
                        <div>
                            <strong>Measurements:</strong>
                            <ul>
                                <li>Measured In: {ingredient.measurementUnit.measuredIn}</li>
                                {ingredient.measurementUnit.weightUnit && (
                                    <li>Weight Unit: {ingredient.measurementUnit.weightUnit}</li>
                                )}
                                {ingredient.measurementUnit.volumeUnit && (
                                    <li>Volume Unit: {ingredient.measurementUnit.volumeUnit}</li>
                                )}
                            </ul>
                        </div>
                    )}
                </CardDescription>
            </CardContent>
            <CardContent extra>
                <Button.Group widths='2'>
                    <Button
                        basic
                        onClick={() => openForm(ingredient.id)}
                        color='teal'
                        content='Edit'
                    />
                    <Button
                        onClick={cancelSelectedIngredient}
                        basic
                        color='grey'
                        content='Cancel'
                    />
                </Button.Group>
            </CardContent>
        </Card>
    )
}
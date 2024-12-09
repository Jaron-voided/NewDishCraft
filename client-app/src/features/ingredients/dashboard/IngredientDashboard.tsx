import {Grid} from "semantic-ui-react";
import {Ingredient} from "../../../app/models/ingredient.ts";
import IngredientList from "./IngredientList.tsx";
import IngredientDetails from "../details/IngredientDetails.tsx";
import IngredientForm from "../form/IngredientForm.tsx";

interface Props {
    ingredients: Ingredient[]; /*// has to have this to accept the ingredient from App.tsx*/
    selectedIngredient: Ingredient | undefined;
    selectIngredient: (id: string) => void;
    cancelSelectIngredient: () => void;
    editMode: boolean;
    openForm: (id: string) => void;
    closeForm: () => void;
    createOrEdit: (ingredient: Ingredient) => void;
    deleteIngredient: (id: string) => void;
    submitting: boolean;
}

export default function IngredientDashboard({ingredients, selectedIngredient,
                                                selectIngredient, cancelSelectIngredient, deleteIngredient,
                                            editMode, openForm, closeForm, submitting, createOrEdit}:
                                            Props) { /*//destructuring ingredients from the props*/
    return (
        <Grid>
           <Grid.Column width='10'>
                <IngredientList
                    ingredients={ingredients}
                    selectIngredient={selectIngredient}
                    deleteIngredient={deleteIngredient}
                    submitting={submitting}
                />
           </Grid.Column>
            <Grid.Column width='6'>
                {selectedIngredient && ! editMode && /*only loads if something exists*/
                <IngredientDetails
                    ingredient={selectedIngredient}
                    cancelSelectIngredient={cancelSelectIngredient}
                    openForm={openForm}
                />}
                {editMode &&
                <IngredientForm
                    closeForm={closeForm}
                    ingredient={selectedIngredient}
                    createOrEdit={createOrEdit}
                    submitting={submitting}
                />}
            </Grid.Column>
        </Grid>
    )
}
/*

<List>
    {ingredients.map((ingredient) => (
        <li key={ingredient.id}>
            <p>{ingredient.name}, {ingredient.category}</p>
            <p>{ingredient.pricePerPackage} {ingredient.measurementsPerPackage}</p>
            {ingredient.nutrition && ( // Check if nutrition exists
                <>
                    <p>Nutrition</p>
                    <p>
                        {ingredient.nutrition.calories} calories,
                        {ingredient.nutrition.carbs} carbs,
                        {ingredient.nutrition.fat} fat,
                        {ingredient.nutrition.protein} protein
                    </p>
                </>
            )}
            {ingredient.measurementUnit && ( // Check if measurementUnit exists
                <>
                    <p>Measurements</p>
                    <p>
                        Measured In: {MeasuredIn[ingredient.measurementUnit.measuredIn]},
                        Weight
                        Unit: {ingredient.measurementUnit.weightUnit !== null && ingredient.measurementUnit.weightUnit !== undefined ? WeightUnit[ingredient.measurementUnit.weightUnit] : "N/A"},
                        Volume
                        Unit: {ingredient.measurementUnit.volumeUnit !== null && ingredient.measurementUnit.volumeUnit !== undefined ? VolumeUnit[ingredient.measurementUnit.volumeUnit] : "N/A"}
                    </p>
                </>
            )}
        </li>
    ))}
</List>*/

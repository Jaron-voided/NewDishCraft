
/*


/!*
    import { useState, useEffect } from "react"; // Removed unused `ChangeEvent` and `SyntheticEvent`
import {
    Button,
    Form,
    Segment,
    Dropdown,
    Header,
    FormField,
} from "semantic-ui-react"; // Removed unused `DropdownProps`
import { Recipe, RecipeCategory } from "../../../app/models/recipe";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { Link, useNavigate, useParams } from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { v4 as uuid } from "uuid";
import { Formik, FormikHelpers } from "formik"; // Added `FormikHelpers` for type hinting in `onSubmit`
import MyTextInput from "../../../app/common/form/MyTextInput";
import MySelectInput from "../../../app/common/form/MySelectInput";
import { ingredientCategoryOptions } from "../../../app/common/options/ingredientCategoryOptions";
import MyNumberInput from "../../../app/common/form/MyNumberInput";
import { measuredInOptions } from "../../../app/common/options/measuredInOptions";
import { MeasuredIn } from "../../../app/models/ingredient";
import { weightOptions } from "../../../app/common/options/weightOptions";
import { volumeOptions } from "../../../app/common/options/volumeOptions";
import MyTextArea from "../../../app/common/form/MyTextArea";

export default observer(function RecipeForm() {
    const { recipeStore } = useStore();
    const {
        createRecipe,
        updateRecipe,
        selectedRecipe,
        loading,
        loadRecipe,
        loadingInitial,
    } = recipeStore;

    interface RecipeFormValues extends Recipe {
        instructionsText: string;
    }


    const { ingredientStore } = useStore();
    const { ingredientsByCategory } = ingredientStore;

    const navigate = useNavigate(); // Added navigation functionality
    const { id } = useParams<{ id: string }>(); // Correct type hint for `useParams`

    const [recipe, setRecipe] = useState<Recipe>({
        id: "",
        name: "",
        recipeCategory: RecipeCategory.Dinner, // Default category
        servingsPerRecipe: 1,
        instructionsJson: "",
        instructions: [],
        measurements: [],
    });

    const [selectedIngredients, setSelectedIngredients] = useState<string[]>([]);
    const [numberOfIngredients, setNumberOfIngredients] = useState(0);

    const [measurements, setMeasurements] = useState(
        Array.from({ length: numberOfIngredients }, () => ({
            recipeId: "",
            ingredientId: "",
            amount: 0,
        }))
    );

    useEffect(() => {
        if (id) {
            loadRecipe(id).then((loadedRecipe) => {
                if (loadedRecipe) {
                    setRecipe(loadedRecipe);
                }
            });
        }
    }, [id, loadRecipe]);

    function handleIngredientAmountChange(value: number) {
        setNumberOfIngredients(value);
    }

    const handleAmountChange = (index: number, amount: number) => {
        const updatedMeasurements = [...recipe.measurements];
        updatedMeasurements[index] = {
            ...updatedMeasurements[index],
            amount,
        };
        setRecipe({ ...recipe, measurements: updatedMeasurements });
    };

    const handleIngredientChange = (index: number, ingredientId: string) => {
        const updatedMeasurements = [...recipe.measurements];
        updatedMeasurements[index] = {
            ...updatedMeasurements[index],
            ingredientId,
        };
        setRecipe({ ...recipe, measurements: updatedMeasurements });
    };


    /!*    function handleAmountChange(index: number, amount: number) {
            setMeasurements((prev) =>
                prev.map((measurement, i) =>
                    i === index
                        ? { ...measurement, amount }
                        : measurement
                )
            );
        }*!/

const [ingredientOptions, setIngredientOptions] = useState<any[]>([]); // State to hold dropdown options

useEffect(() => {
    ingredientStore.loadIngredients(); // Trigger data loading
}, []);

// Populate ingredientOptions when ingredientsByCategory updates
useEffect(() => {
    console.log("IngredientsByCategory:", ingredientsByCategory);
    if (ingredientsByCategory) {
        const options = ingredientsByCategory.map((ingredient) => ({
            key: ingredient.id,
            text: ingredient.name,
            value: ingredient.id,
        }));
        setIngredientOptions(options);
    }
}, [ingredientsByCategory]);


/!*    // Handle specific ingredient selection
    const handleIngredientChange = (index: number, value: string) => {
        setSelectedIngredients((prev) => {
            const updated = [...prev];
            updated[index] = value;
            return updated;
        });
    };*!/


const onSubmit = (
    values: RecipeFormValues,
    { setSubmitting }: FormikHelpers<RecipeFormValues>
) => {
    const updatedRecipe: Recipe = {
        ...values,
        instructions: values.instructionsText.split("\n").map((line) => line.trim()), // Convert multi-line string to array
        instructionsJson: JSON.stringify(values.instructions), // Convert array to JSON string
    };

    if (!values.id) {
        updatedRecipe.id = uuid(); // Assign a new ID if creating a new recipe
        console.log("Payload being sent:", updatedRecipe);
        createRecipe(updatedRecipe).then(() => navigate(`/recipes/${updatedRecipe.id}`));
    } else {
        console.log("Payload being sent:", updatedRecipe);
        updateRecipe(updatedRecipe).then(() => navigate(`/recipes/${updatedRecipe.id}`));
    }
    setSubmitting(false);
};

/!*    const onSubmit = (values: Recipe, { setSubmitting }: FormikHelpers<Recipe>) => {
        const updatedRecipe = {
            ...values,
            instructionsJson: JSON.stringify(values.instructions), // Convert array to JSON string
        };

        if (!values.id) {
            updatedRecipe.id = uuid(); // Assign a new ID if creating a new recipe
            console.log("Payload being sent:", values);
            createRecipe(updatedRecipe).then(() => navigate(`/recipes/${updatedRecipe.id}`));
        } else {
            console.log("Payload being sent:", values);
            updateRecipe(updatedRecipe).then(() => navigate(`/recipes/${updatedRecipe.id}`));
        }
        setSubmitting(false);
    };*!/


/!*    const onSubmit = async (
        values: Recipe,
        { setSubmitting }: FormikHelpers<Recipe>
    ) => {
        try {
            if (!values.id) {
                values.id = uuid();
                console.log("Payload being sent:", values);
                await createRecipe(values);
            } else {
                console.log("Payload being sent:", values);
                await updateRecipe(values);
            }
            navigate(`/recipes/${values.id}`);
        } catch (error) {
            console.error("Error submitting recipe form:", error);
        } finally {
            setSubmitting(false);
        }
    };*!/

if (loadingInitial) return <LoadingComponent content="Loading recipe..." />;

return (
    <Segment clearing>
        <Header as="h2" content={id ? "Edit Recipe" : "Create Recipe"} />
        <Formik<RecipeFormValues>
            enableReinitialize
            initialValues={{
                ...recipe,
                instructionsText: recipe.instructions.join("\n"), // Convert array to multi-line string
            }}
            onSubmit={onSubmit}
        >
            {({ handleSubmit }) => (
                <Form className="ui form" onSubmit={handleSubmit}>
                    <MyTextInput
                        placeholder="Recipe Name"
                        name="name"
                        label="Recipe Name"
                    />
                    <MySelectInput
                        placeholder="Category"
                        name="recipeCategory"
                        options={Object.values(RecipeCategory).map(
                            (category) => ({
                                key: category,
                                text: category,
                                value: category,
                            })
                        )}
                    />
                    <MyNumberInput
                        placeholder="Servings"
                        name="servingsPerRecipe"
                        label="Servings Per Recipe"
                    />
                    <MyTextArea
                        placeholder="Enter instructions here"
                        name="instructions"
                        rows={recipe.instructions.length || 3} // Default to 3 rows if no instructions
                    />

                    <Form.Field>
                        <label>How many ingredients</label>
                        <Form.Input
                            placeholder="Number of ingredients"
                            type="number"
                            value={numberOfIngredients}
                            onChange={(_e, data ) =>
                                handleIngredientAmountChange(Number(data.value))}
                        />
                    </Form.Field>
                    {Array.from({ length: numberOfIngredients }).map((_, index) => (
                        <Form.Group key={index}>
                            <Form.Input
                                placeholder="Enter amount"
                                type="number"
                                onChange={(e) =>
                                    handleAmountChange(index, Number(e.target.value))}
                            />
                            <Dropdown
                                placeholder="Select Ingredients"
                                fluid
                                selection
                                options={ingredientOptions}
                                value={selectedIngredients[index] || ''}
                                //value={selectedIngredient || ''}
                                onChange={(_e, { value }) =>
                                    handleIngredientChange(index, String(value))}
                            />
                        </Form.Group>
                    ))}
                    <Button
                        floated="right"
                        positive
                        type="submit"
                        content="Submit"
                        loading={loading}
                    />
                    <Button
                        floated="right"
                        type="button"
                        content="Cancel"
                        as={Link}
                        to="/recipes"
                    />
                </Form>
            )}
        </Formik>
    </Segment>
);
});

*!/
*/

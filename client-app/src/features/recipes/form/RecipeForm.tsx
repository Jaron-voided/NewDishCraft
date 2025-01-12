import {useState, ChangeEvent, SyntheticEvent, useEffect} from "react";
import {Button, Form, Segment, Dropdown, DropdownProps} from "semantic-ui-react";
import {Recipe, RecipeCategory} from "../../../app/models/recipe.ts";
import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";
import {Link, useNavigate, useParams} from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";
import {v4 as uuid} from "uuid";

export default observer(function RecipeForm() {
    const {recipeStore} = useStore();
    const {
        createRecipe, updateRecipe, loading, loadRecipe, loadingInitial
    } = recipeStore;

    const [selectedIngredients, setSelectedIngredients] = useState<string[]>([]);
    const {ingredientStore} = useStore();
    const {ingredientsByCategory} = ingredientStore;

    const [numberOfIngredients, setNumberOfIngredients] = useState(0);

    const [measurements, setMeasurements] = useState<
        { recipeId: string; ingredientId: string; amount: number }[]
    >(
        Array.from({ length: numberOfIngredients }, () => ({
            recipeId: '', // Placeholder for recipe ID
            ingredientId: '', // Placeholder for ingredient ID
            amount: 0,
        }))
    );


    const {id} = useParams();
    const navigate = useNavigate();


    const [recipe, setRecipe] = useState<Recipe>({
        id: '',
        name: '',
        recipeCategory: RecipeCategory.Soup, // Default to Spice
        servingsPerRecipe: 0,
        instructionsJson: '',
        instructions: [],
        measurements: []
    })

    const ingredientOptions = ingredientStore.ingredientsByCategory.map((ingredient) => ({
        key: ingredient.id,
        text: ingredient.name,
        value: ingredient.id,
    }));

    useEffect(() => {
        setMeasurements(
            Array.from({ length: numberOfIngredients }, () => ({
                recipeId: recipe.id || '',
                ingredientId: '',
                amount: 0,
            }))
        );
        console.log('Measurements state updated:', measurements);
    }, [numberOfIngredients, recipe.id]);


    useEffect(() => {
        if (id) {
            loadRecipe(id).then(recipe => {
                setRecipe(recipe!);
                setNumberOfIngredients(recipe!.measurements.length);
                setMeasurements(recipe!.measurements);
            });
        }
    }, [id, loadRecipe]);

    useEffect(() => {
        if (ingredientsByCategory.length === 0) {
            ingredientStore.loadIngredients();
        }
    }, [ingredientStore]);

    function handleIngredientAmountChange(value: number) {
        setNumberOfIngredients(value);
    }

    function handleAmountChange(index: number, amount: number) {
        setMeasurements((prev) =>
            prev.map((measurement, i) =>
                i === index
                    ? { ...measurement, amount }
                    : measurement
            )
        );
        console.log(`Updated measurement at index ${index}:`, { ...measurements[index], amount });
    }

    function handleIngredientChange(index: number, value: string | number) {
        const ingredientId = String(value);
        setSelectedIngredients((prev) => {
            const updated = [...prev];
            updated[index] = ingredientId;
            return updated;
        });

        setMeasurements((prev) =>
            prev.map((measurement, i) =>
                i === index ? { ...measurement, ingredientId } : measurement
            )
        );

        console.log('Updated measurements:', measurements);
    }


    function handleSubmit() {
        // Validate measurements

        if (!recipe.id) {
            recipe.id = uuid();
            console.log('Payload being sent (Create):', measurements);
            measurements.forEach((m) => (m.recipeId = recipe.id)); // Attach recipe ID
            createRecipe(recipe, measurements)
                .then(() => navigate(`/recipes/${recipe.id}`))
                .catch((error) => console.error("Error creating recipe:", error));
        } else {
            console.log('Payload being sent (Update):', measurements);
            updateRecipe(recipe)
                .then(() => navigate(`/recipes/${recipe.id}`))
                .catch((error) => console.error("Error updating recipe:", error));
        }
    }

    function handleInputChange(event: ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) {
        const { name, value } = event.target;

        if (name === "instructions") {
            setRecipe({
                ...recipe,
                instructions: value.split("\n"), // Split multiline input into an array
                instructionsJson: value.replace(/\n/g, ";"), // Convert multiline input to JSON string format
            });
        } else {
            setRecipe({
                ...recipe,
                [name]: value,
            });
        }
    }


    function handleDropdownChange(
        _event: SyntheticEvent<HTMLElement>,
        data : DropdownProps
    ) {
        setRecipe({
            ...recipe,
            recipeCategory: data.value as RecipeCategory, // Type assertion to RecipeCategory
        });
    }

    // Dropdown options for categories
    const categoryOptions = Object.values(RecipeCategory).map((recipeCategory) => ({
        key: recipeCategory,
        text: recipeCategory,
        value: recipeCategory,
    }));


    if (loadingInitial) return <LoadingComponent content='Loading Recipe...' />

    return (
        <Segment clearing>
            <Form onSubmit={handleSubmit} autoComplete="off">
                {/* Name of Recipe */}
                <Form.Field>
                    <label>Name of Recipe</label>
                    <Form.Input
                        placeholder="Name"
                        value={recipe.name}
                        name="name"
                        onChange={handleInputChange}
                    />
                </Form.Field>

                {/* Recipe Category */}
                <Form.Field>
                    <label>Recipe Category</label>
                    <Dropdown
                        placeholder="Select Category"
                        fluid
                        selection
                        options={categoryOptions}
                        name="recipeCategory"
                        value={recipe.recipeCategory}
                        onChange={handleDropdownChange}
                    />
                </Form.Field>

                {/* Price Per Package */}
                <Form.Field>
                    <label>Servings Per Recipe</label>
                    <Form.Input
                        placeholder="Servings Per Recipe"
                        type="number"
                        value={recipe.servingsPerRecipe}
                        name="servingsPerRecipe"
                        onChange={handleInputChange}
                    />
                </Form.Field>

                {/* Measurements in Package */}
                <Form.Field>
                    <label>Instructions</label>
                    <Form.TextArea
                        placeholder="Enter instructions here"
                        value={recipe.instructions.join("\n")} // Convert array to multiline string for display
                        name="instructions"
                        onChange={handleInputChange}
                    />
                </Form.Field>

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



                {/* Submit and Cancel Buttons */}
                <Button
                    loading={loading}
                    floated="right"
                    positive
                    type="submit"
                    content="Submit"
                />
                <Button
                    as={Link} to='/recipes'
                    floated="right"
                    type="button"
                    content="Cancel"
                />
            </Form>

        </Segment>
    );
})


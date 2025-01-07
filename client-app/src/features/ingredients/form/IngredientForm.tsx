import {useState, ChangeEvent, FormEvent, SyntheticEvent, useEffect} from "react";
import {Button, Form, Segment, Dropdown, DropdownProps, CheckboxProps} from "semantic-ui-react";
import {Ingredient, IngredientCategory, MeasuredIn, VolumeUnit, WeightUnit} from "../../../app/models/ingredient.ts";
import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";
import {Link, useNavigate, useParams} from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";
import {v4 as uuid} from "uuid";

export default observer(function IngredientForm() {
    const {ingredientStore} = useStore();
    const {
        createIngredient, updateIngredient, loading, loadIngredient, loadingInitial
    } = ingredientStore;

    const {id} = useParams();
    const navigate = useNavigate();

    const [ingredient, setIngredient] = useState<Ingredient>({
        id: '',
        name: '',
        category: IngredientCategory.Spice, // Default to Spice
        pricePerPackage: 0,
        measurementsPerPackage: 0,
        measurementUnit: {
            ingredientId: '', // Default to empty string
            measuredIn: MeasuredIn.Weight, // Default to Weight
            weightUnit: undefined,
            volumeUnit: undefined,
        },
        nutrition: {
            ingredientId: '', // Default to empty string
            calories: 0,
            protein: 0,
            carbs: 0,
            fat: 0,
        },
    })


    useEffect(() => {
        if (id) {
            loadIngredient(id).then(ingredient => setIngredient(ingredient!));
        }
    }, [id, loadIngredient]);

    function handleSubmit() {
        if (!ingredient.id) {
            ingredient.id = uuid();
            createIngredient(ingredient).then(() => navigate(`/ingredients/${ingredient.id}`))
        } else {
            updateIngredient(ingredient).then(() => navigate(`/ingredients/${ingredient.id}`))
        }
    }

    function handleInputChange(event: ChangeEvent<HTMLInputElement>) {
        const { name, value } = event.target;

        if (['calories', 'protein', 'carbs', 'fat'].includes(name)) {
            setIngredient({
                ...ingredient,
                nutrition: {
                    ingredientId: ingredient.nutrition!.ingredientId, // Preserve the ingredientId
                    calories: ingredient.nutrition!.calories || 0,   // Ensure calories is not undefined
                    carbs: ingredient.nutrition!.carbs || 0,         // Ensure carbs is not undefined
                    fat: ingredient.nutrition!.fat || 0,             // Ensure fat is not undefined
                    protein: ingredient.nutrition!.protein || 0,     // Ensure protein is not undefined
                    [name]: parseFloat(value),                      // Update the specific field
                },
            });
        } else {
            setIngredient({
                ...ingredient,
                [name]: value,
            });
        }
    }



    function handleDropdownChange(
        _event: SyntheticEvent<HTMLElement>,
        data : DropdownProps
    ) {
        setIngredient({
            ...ingredient,
            category: data.value as IngredientCategory, // Type assertion to IngredientCategory
        });
    }


    function handleRadioChange(
        _event: FormEvent<HTMLInputElement>,
        { value }: CheckboxProps
    ) {
        if (value !== undefined && typeof value === "string") {
            setIngredient({
                ...ingredient,
                measurementUnit: {
                    ...ingredient.measurementUnit!,
                    measuredIn: value as MeasuredIn, // Type assertion to MeasuredIn
                    weightUnit: value === MeasuredIn.Weight ? null : undefined, // Reset WeightUnit if applicable
                    volumeUnit: value === MeasuredIn.Volume ? null : undefined, // Reset VolumeUnit if applicable

                },
            });
        }
    }

    // Dropdown options for categories
    const categoryOptions = Object.values(IngredientCategory).map((category) => ({
        key: category,
        text: category,
        value: category,
    }));

    // Dropdown options for WeightUnit
    const weightUnitOptions = Object.values(WeightUnit).map((unit) => ({
        key: unit,
        text: unit,
        value: unit,
    }));

// Dropdown options for VolumeUnit
    const volumeUnitOptions = Object.values(VolumeUnit).map((unit) => ({
        key: unit,
        text: unit,
        value: unit,
    }));

    if (loadingInitial) return <LoadingComponent content='Loading Ingredient...' />

    return (
        <Segment clearing>
            <Form onSubmit={handleSubmit} autoComplete="off">
                {/* Name of Ingredient */}
                <Form.Field>
                    <label>Name of Ingredient</label>
                    <Form.Input
                        placeholder="Name"
                        value={ingredient.name}
                        name="name"
                        onChange={handleInputChange}
                    />
                </Form.Field>

                {/* Ingredient Category */}
                <Form.Field>
                    <label>Ingredient Category</label>
                    <Dropdown
                        placeholder="Select Category"
                        fluid
                        selection
                        options={categoryOptions}
                        name="category"
                        value={ingredient.category}
                        onChange={handleDropdownChange}
                    />
                </Form.Field>

                {/* Price Per Package */}
                <Form.Field>
                    <label>Price Per Package</label>
                    <Form.Input
                        placeholder="Price Per Package"
                        type="number"
                        value={ingredient.pricePerPackage}
                        name="pricePerPackage"
                        onChange={handleInputChange}
                    />
                </Form.Field>

                {/* Measurements in Package */}
                <Form.Field>
                    <label>Measurements in Package</label>
                    <Form.Input
                        placeholder="Measurements Per Package"
                        type="number"
                        value={ingredient.measurementsPerPackage}
                        name="measurementsPerPackage"
                        onChange={handleInputChange}
                    />
                </Form.Field>

                {/* Nutritional Values */}
                <Form.Field>
                    <label>Nutritional Values</label>
                </Form.Field>
                <Form.Field>
                    <label>Calories</label>
                    <Form.Input
                        placeholder="Calories"
                        type="number"
                        value={ingredient.nutrition!.calories}
                        name="calories"
                        onChange={handleInputChange}
                    />
                </Form.Field>
                <Form.Field>
                    <label>Protein (g)</label>
                    <Form.Input
                        placeholder="Protein (g)"
                        type="number"
                        value={ingredient.nutrition!.protein}
                        name="protein"
                        onChange={handleInputChange}
                    />
                </Form.Field>
                <Form.Field>
                    <label>Carbohydrates (g)</label>
                    <Form.Input
                        placeholder="Carbohydrates (g)"
                        type="number"
                        value={ingredient.nutrition!.carbs}
                        name="carbs"
                        onChange={handleInputChange}
                    />
                </Form.Field>
                <Form.Field>
                    <label>Fat (g)</label>
                    <Form.Input
                        placeholder="Fat (g)"
                        type="number"
                        value={ingredient.nutrition!.fat}
                        name="fat"
                        onChange={handleInputChange}
                    />
                </Form.Field>

                {/* Measured In */}
                <Form.Field>
                    <label>Measured In</label>
                    <Form.Group grouped>
                        {Object.values(MeasuredIn).map((measuredIn) => (
                            <Form.Radio
                                key={measuredIn}
                                label={measuredIn}
                                value={measuredIn}
                                checked={ingredient.measurementUnit?.measuredIn === measuredIn}
                                onChange={handleRadioChange}
                            />
                        ))}
                    </Form.Group>
                </Form.Field>

                {/* Weight Unit */}
                {ingredient.measurementUnit?.measuredIn === MeasuredIn.Weight && (
                    <Form.Field>
                        <label>Weight Unit</label>
                        <Dropdown
                            placeholder="Select Weight Unit"
                            fluid
                            selection
                            options={weightUnitOptions}
                            name="weightUnit"
                            value={ingredient.measurementUnit.weightUnit || ''}
                            onChange={(_e, { value }) =>
                                setIngredient({
                                    ...ingredient,
                                    measurementUnit: {
                                        ...ingredient.measurementUnit!,
                                        weightUnit: value as WeightUnit,
                                    },
                                })
                            }
                        />
                    </Form.Field>
                )}

                {/* Volume Unit */}
                {ingredient.measurementUnit?.measuredIn === MeasuredIn.Volume && (
                    <Form.Field>
                        <label>Volume Unit</label>
                        <Dropdown
                            placeholder="Select Volume Unit"
                            fluid
                            selection
                            options={volumeUnitOptions}
                            name="volumeUnit"
                            value={ingredient.measurementUnit.volumeUnit || ''}
                            onChange={(_e, { value }) =>
                                setIngredient({
                                    ...ingredient,
                                    measurementUnit: {
                                        ...ingredient.measurementUnit!,
                                        volumeUnit: value as VolumeUnit,
                                    },
                                })
                            }
                        />
                    </Form.Field>
                )}

                {/* Submit and Cancel Buttons */}
                <Button
                    loading={loading}
                    floated="right"
                    positive
                    type="submit"
                    content="Submit"
                />
                <Button
                    as={Link} to='/ingredients'
                    floated="right"
                    type="button"
                    content="Cancel"
                />
            </Form>
        </Segment>
    );
})

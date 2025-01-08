//import {useState, ChangeEvent, FormEvent, SyntheticEvent, useEffect} from "react";
import {Button, Segment, Dropdown, FormField, Header} from "semantic-ui-react";
import {Ingredient, IngredientCategory, MeasuredIn, VolumeUnit, WeightUnit} from "../../../app/models/ingredient.ts";
import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";
import {Link, useNavigate} from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";
import {v4 as uuid} from "uuid";
import {Formik, Form, Field, ErrorMessage} from "formik";
import {values} from "mobx";
import * as Yup from "yup";
import MyTextInput from "../../../app/common/form/MyTextInput.tsx";
import MySelectInput from "../../../app/common/form/MySelectInput.tsx";
import {ingredientCategoryOptions} from "../../../app/common/options/ingredientCategoryOptions.ts";
import {weightOptions} from "../../../app/common/options/weightOptions.ts";
import {volumeOptions} from "../../../app/common/options/volumeOptions.ts";
import MyNumberInput from "../../../app/common/form/MyNumberInput.tsx";
import {measuredInOptions} from "../../../app/common/options/measuredInOptions.ts";


export default observer(function IngredientForm() {
    const {ingredientStore} = useStore();
    const { createIngredient, updateIngredient, selectedIngredient,/*loading, loadIngredient,*/ loadingInitial } = ingredientStore;

    //const { id } = useParams();
    const navigate = useNavigate();

    // Validation Schema
    const validationSchema = Yup.object({
        name: Yup.string().required("Name is required"),
        category: Yup.string().required("Category is required"),
        pricePerPackage: Yup.number()
            .min(0, "Price must be positive")
            .required("Price is required"),
        measurementsPerPackage: Yup.number()
            .min(0, "Measurements must be positive")
            .required("Measurements are required"),
        measurementUnit: Yup.object({
            ingredientId: Yup.string().required("Ingredient ID is required"),
        }),
        nutrition: Yup.object({
            ingredientId: Yup.string().required("Ingredient ID is required"),
            calories: Yup.number()
                .min(0, "Calories must be positive")
                .required("Calories are required"),
            protein: Yup.number()
                .min(0, "Protein must be positive")
                .required("Protein is required"),
            carbs: Yup.number()
                .min(0, "Carbs must be positive")
                .required("Carbs are required"),
            fat: Yup.number()
                .min(0, "Fat must be positive")
                .required("Fat is required"),
        }),
    });

    interface IngredientFormValues {
        id: string;
        name: string;
        category: IngredientCategory;
        pricePerPackage: number;
        measurementsPerPackage: number;
        measurementUnit: {
            ingredientId: string;
            measuredIn: MeasuredIn;
            weightUnit?: WeightUnit;
            volumeUnit?: VolumeUnit;
        };
        nutrition: {
            ingredientId: string;
            calories: number;
            protein: number;
            carbs: number;
            fat: number;
        };
    }

    const initialIngredient = selectedIngredient ?? {
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
    };

    const unitOptions2 = {
        [MeasuredIn.Weight]: Object.values(WeightUnit).map((unit) => ({
            key: unit,
            text: unit,
            value: unit,
        })),
        [MeasuredIn.Volume]: Object.values(VolumeUnit).map((unit) => ({
            key: unit,
            text: unit,
            value: unit,
        })),
    };

    // Dropdown options for categories
    const categoryOptions2 = Object.values(IngredientCategory).map((category) => ({
        key: category,
        text: category,
        value: category,
    }));

    const measuredInOptions2 = Object.values(MeasuredIn).map((measuredIn) => ({
        key: measuredIn,
        text: measuredIn,
        value: measuredIn,
    }));


/*    function handleFormSubmit(ingredient: Ingredient) {
        if (ingredient.id.length === 0) {
            let newIngredient = {
                ...ingredient,
                id: uuid()
            };
            createIngredient(newIngredient).then(() => navigate(`/ingredients/${newIngredient.id}`));
        } else {
            updateIngredient(ingredient).then(() => navigate(`/ingredients/${ingredient.id}`));
        }
    }*/

 /*   function onSubmit(values: IngredientFormValues) {
        const ingredientToSubmit: Ingredient = {
            ...values,
            id: values.id || uuid(), // Generate a new ID if not present
            measurementUnit: {
                ...values.measurementUnit,
                ingredientId: values.id || uuid(), // Ensure `ingredientId` is set
            },
            nutrition: {
                ...values.nutrition,
                ingredientId: values.id || uuid(), // Ensure `ingredientId` is set
            },
        };

        if (!values.id) {
            // New ingredient creation
            createIngredient(ingredientToSubmit).then(() =>
                navigate(`/ingredients/${ingredientToSubmit.id}`)
            );
        } else {
            // Update existing ingredient
            updateIngredient(ingredientToSubmit).then(() =>
                navigate(`/ingredients/${ingredientToSubmit.id}`)
            );
        }
    }*/
    const onSubmit = async (values: typeof initialIngredient) => {
        console.log("Formik onSubmit triggered");
        try {
            if (!values.id) {
                const newId = uuid();
                const newIngredient = {
                    ...values,
                    id: newId,
                    measurementUnit: {
                        ...values.measurementUnit,
                        ingredientId: newId,
                    },
                    nutrition: {
                        ...values.nutrition,
                        ingredientId: newId,
                    },
                };
                console.log("Creating ingredient:", newIngredient);
                await createIngredient(newIngredient);
                navigate(`/ingredients/${newIngredient.id}`);
            } else {
                console.log("Updating ingredient:", values);
                await updateIngredient(values);
                navigate(`/ingredients/${values.id}`);
            }
        } catch (error) {
            console.error("Error in onSubmit:", error);
        } finally {
            console.log("Submission completed");
        }
    };

       /* const ingredientToSubmit = {
            ...values,
            measurementUnit: {
                ...values.measurementUnit,
                ingredientId: values.id || uuid(),
            },
            nutrition: {
                ...values.nutrition,
                ingredientId: values.id || uuid(),
            },
        };

        if (!values.id) {
            ingredientToSubmit.id = uuid();
            createIngredient(ingredientToSubmit).then(() =>
                navigate(`/ingredients/${ingredientToSubmit.id}`)
            );
        } else {
            updateIngredient(ingredientToSubmit).then(() =>
                navigate(`/ingredients/${ingredientToSubmit.id}`)
            );
        }
    };
*/
    /*
        const onSubmit = (values: IngredientFormValues) => {
            const ingredientToSubmit = {
                ...values,
                measurementUnit: {
                    ...values.measurementUnit,
                    ingredientId: values.id || uuid(),
                },
                nutrition: {
                    ...values.nutrition,
                    ingredientId: values.id || uuid(),
                },
            };

            if (!values.id) {
                ingredientToSubmit.id = uuid();
                createIngredient(ingredientToSubmit).then(() =>
                    navigate(`/ingredients/${ingredientToSubmit.id}`)
                );
            } else {
                updateIngredient(ingredientToSubmit).then(() =>
                    navigate(`/ingredients/${ingredientToSubmit.id}`)
                );
            }
        };
    */

    if (loadingInitial) return <LoadingComponent content='Loading Ingredient...' />

    return (
        <Segment clearing>
            <Header content='Ingredient Form' sub color='teal' />
{/*
            <Formik<IngredientFormValues>
*/}
            <Formik
                //validationSchema={validationSchema}
                //enableReinitialize
                initialValues={initialIngredient}
                onSubmit={(values) => {
                    console.log("Formik onSubmit triggered");
                    onSubmit(values);
                }}
            >
                {({values, handleChange, handleSubmit, setFieldValue, isSubmitting, isValid, dirty}) => (
                    <Form
                        className="ui form"
                        onSubmit={(e) => {
                            console.log("Form Submitted!");
                            handleSubmit(e);
                        }}
                    >
                        <FormField>
                            <MyTextInput name="name" placeholder="Name" />
{/*                            <Field placeholder="Name" name="name" />
                            <ErrorMessage name="name" component="div" className="ui pointing red basic label" />*/}
                        </FormField>

                        <FormField>
                            <MySelectInput options={categoryOptions2} placeholder='Category' name='category' />
{/*                            <Dropdown
                                placeholder="Category"
                                selection
                                options={categoryOptions}
                                name="category"
                                value={ingredient.category}
                                onChange={(_e, { value }) => setFieldValue("category", value)}
                            />
                            <ErrorMessage name="category" component="div" className="ui pointing red basic label" />*/}
                        </FormField>

                        <FormField>
                            <MyNumberInput placeholder='Price Per Package' name='pricePerPackage' label='Price Per Package' min={0} step={0.01} />
{/*                            <Field placeholder="Price per Package" name="pricePerPackage" type="number" />
                            <ErrorMessage name="pricePerPackage" component="div" className="ui pointing red basic label" />*/}
                        </FormField>

                        <FormField>
                            <MyNumberInput placeholder='Measurements Per Package' name='measurementsPerPackage' label='Measurements Per Package' min={0} />
{/*                            <Field placeholder="Measurements per Package" name="measurementsPerPackage" type="number" />
                            <ErrorMessage
                                name="measurementsPerPackage"
                                component="div"
                                className="ui pointing red basic label"
                            />*/}
                        </FormField>

                        <FormField>
                            <MySelectInput options={measuredInOptions2} placeholder='Measured In' name='measurementUnit.measuredIn' />
{/*                            <Dropdown
                                placeholder="Measured In"
                                selection
                                options={measuredInOptions}
                                name="measurementUnit.measuredIn"
                                value={ingredient.measurementUnit.measuredIn}
                                onChange={(_e, { value }) => setFieldValue("measurementUnit.measuredIn", value)}
                            />*/}

                            {values.measurementUnit.measuredIn === MeasuredIn.Weight && (
                                <MySelectInput placeholder='Weight Unit' name='measurementUnit.weightUnit' options={unitOptions2[MeasuredIn.Weight]} />

                                /*             <Dropdown
                                    placeholder="Weight Unit"
                                    selection
                                    options={unitOptions[MeasuredIn.Weight]}
                                    name="measurementUnit.weightUnit"
                                    value={ingredient.measurementUnit.weightUnit}
                                    onChange={(_e, { value }) => setFieldValue("measurementUnit.weightUnit", value)}
                                />*/
                            )}

                            {values.measurementUnit.measuredIn === MeasuredIn.Volume && (
                                <MySelectInput placeholder='Volume Unit' name='measurementUnit.volumeUnit' options={unitOptions2[MeasuredIn.Volume]} />
                                /*                           <Dropdown
                                    placeholder="Volume Unit"
                                    selection
                                    options={unitOptions[MeasuredIn.Volume]}
                                    name="measurementUnit.volumeUnit"
                                    value={ingredient.measurementUnit.volumeUnit}
                                    onChange={(_e, { value }) => setFieldValue("measurementUnit.volumeUnit", value)}
                                />*/
                            )}
                        </FormField>

                        {/* Nutrition Section */}
                        <FormField>
                            <MyNumberInput placeholder='Calories' name='nutrition.calories' label='Calories' min={0} />
                            <MyNumberInput placeholder='Protein' name='nutrition.protein' label='Protein' min={0} />
                            <MyNumberInput placeholder='Carbs' name='nutrition.carbs' label='Carbs' min={0} />
                            <MyNumberInput placeholder='Fat' name='nutrition.fat' label='Fat' min={0} />
{/*                            <FormField>
                                <Field placeholder="Calories" name="nutrition.calories" type="number" />
                                <ErrorMessage name="nutrition.calories" component="div" className="ui pointing red basic label" />
                            </FormField>

                            <FormField>
                                <Field placeholder="Protein" name="nutrition.protein" type="number" />
                                <ErrorMessage name="nutrition.protein" component="div" className="ui pointing red basic label" />
                            </FormField>
                            <FormField>
                                <Field placeholder="Carbs" name="nutrition.carbs" type="number" />
                                <ErrorMessage name="nutrition.carbs" component="div" className="ui pointing red basic label" />
                            </FormField>
                            <FormField>
                                <Field placeholder="Fat" name="nutrition.fat" type="number" />
                                <ErrorMessage name="nutrition.fat" component="div" className="ui pointing red basic label" />
                            </FormField>*/}
                        </FormField>

                        <Button
                            //disabled={isSubmitting || !dirty || !isValid }
                            positive
                            type="submit"
                            content="Submit"
                            loading={isSubmitting}
                            floated="right"
                        />
                        <Button
                            as={Link}
                            to="/ingredients"
                            type="button"
                            content="Cancel"
                            floated="right"
                        />
                    </Form>
                )}
            </Formik>
        </Segment>
    );
});

import {Button, Segment, FormField, Header} from "semantic-ui-react";
import { IngredientCategory, MeasuredIn } from "../../../app/models/ingredient.ts";
import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";
import {Link, useNavigate, useParams} from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";
import {v4 as uuid} from "uuid";
import {Formik, Form} from "formik";
//import {values} from "mobx";
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
  /*  const validationSchema = Yup.object({
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
    });*/


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


    if (loadingInitial) return <LoadingComponent content='Loading Ingredient...' />

    return (
        <Segment clearing>
            <Header content='Ingredient Form' sub color='teal' />

            <Formik
                //validationSchema={validationSchema} //does not submit??
                enableReinitialize
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
                        </FormField>

                        <FormField>
                            <MySelectInput options={ingredientCategoryOptions} placeholder='Category' name='category' />
                        </FormField>

                        <FormField>
                            <MyNumberInput placeholder='Price Per Package' name='pricePerPackage' label='Price Per Package' min={0} step={0.01} />
                        </FormField>

                        <FormField>
                            <MyNumberInput placeholder='Measurements Per Package' name='measurementsPerPackage' label='Measurements Per Package' min={0} />
                        </FormField>

                        <FormField>
                            <MySelectInput options={measuredInOptions} placeholder='Measured In' name='measurementUnit.measuredIn' />

                            {values.measurementUnit.measuredIn === MeasuredIn.Weight && (
                                <MySelectInput placeholder='Weight Unit' name='measurementUnit.weightUnit' options={weightOptions} />
                            )}

                            {values.measurementUnit.measuredIn === MeasuredIn.Volume && (
                                <MySelectInput placeholder='Volume Unit' name='measurementUnit.volumeUnit' options={volumeOptions} />
                            )}
                        </FormField>

                        {/* Nutrition Section */}
                        <FormField>
                            <MyNumberInput placeholder='Calories' name='nutrition.calories' label='Calories' min={0} />
                            <MyNumberInput placeholder='Protein' name='nutrition.protein' label='Protein' min={0} />
                            <MyNumberInput placeholder='Carbs' name='nutrition.carbs' label='Carbs' min={0} />
                            <MyNumberInput placeholder='Fat' name='nutrition.fat' label='Fat' min={0} />
                        </FormField>

                        <Button
                            disabled={isSubmitting || !dirty || !isValid }
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

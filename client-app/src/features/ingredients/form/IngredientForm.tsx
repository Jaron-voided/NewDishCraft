import { Button, Segment, FormField, Header } from "semantic-ui-react";
import { Ingredient, IngredientCategory, MeasuredIn } from "../../../app/models/ingredient.ts";
import { useStore } from "../../../app/stores/store.ts";
import { observer } from "mobx-react-lite";
import { Link, useNavigate } from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";
import { v4 as uuid } from "uuid";
import { Formik, Form } from "formik";
import MyTextInput from "../../../app/common/form/MyTextInput.tsx";
import MySelectInput from "../../../app/common/form/MySelectInput.tsx";
import { ingredientCategoryOptions } from "../../../app/common/options/ingredientCategoryOptions.ts";
import { weightOptions } from "../../../app/common/options/weightOptions.ts";
import { volumeOptions } from "../../../app/common/options/volumeOptions.ts";
import MyNumberInput from "../../../app/common/form/MyNumberInput.tsx";
import { measuredInOptions } from "../../../app/common/options/measuredInOptions.ts";
import styles from "./IngredientForm.module.css";

export default observer(function IngredientForm() {
    const { ingredientStore } = useStore();
    const { createIngredient, updateIngredient, selectedIngredient, loadingInitial } = ingredientStore;

    const navigate = useNavigate();

    const initialIngredient = selectedIngredient ?? {
        id: '',
        name: '',
        category: IngredientCategory.Spice,
        pricePerPackage: 0,
        measurementsPerPackage: 0,
        measurementUnit: {
            ingredientId: '',
            measuredIn: MeasuredIn.Weight,
            weightUnit: undefined,
            volumeUnit: undefined,
        },
        nutrition: {
            ingredientId: '',
            calories: 0,
            protein: 0,
            carbs: 0,
            fat: 0,
        },
    };

    const onSubmit = async (values: Ingredient, {resetForm: resetForm}) => {
        try {
            if (!values.id) {
                const newId = uuid();
                const newIngredient: Ingredient = {
                    ...values,
                    id: newId,
                    measurementUnit: {
                        ingredientId: newId,
                        measuredIn: values.measurementUnit.measuredIn || MeasuredIn.Weight,
                        weightUnit: values.measurementUnit?.weightUnit,
                        volumeUnit: values.measurementUnit?.volumeUnit
                    },
                    nutrition: {
                        ingredientId: newId,
                        calories: values.nutrition?.calories || 0,
                        protein: values.nutrition?.protein || 0,
                        carbs: values.nutrition?.carbs || 0,
                        fat: values.nutrition?.fat || 0
                    }
                };
                await createIngredient(newIngredient);
                resetForm();
                navigate(`/ingredients`);
            } else {
                await updateIngredient(values);
                navigate(`/ingredients`);
            }
        } catch (error) {
            console.error("Error in onSubmit:", error);
        }
    };

    if (loadingInitial) return <LoadingComponent content="Loading Ingredient..." />;

    return (
        <Segment clearing>
            <Header as="h2" textAlign="center" color="teal" className={styles.header}>
                Ingredient Form
            </Header>
            <Formik
                enableReinitialize
                initialValues={initialIngredient}
                onSubmit={onSubmit}
            >
                {({ values, handleSubmit, isSubmitting, isValid, dirty }) => (
                    <Form className={`ui form ${styles.form}`} onSubmit={handleSubmit}>
                        <div className={styles.row}>
                            <FormField className={styles.field}>
                                <MyTextInput name="name" placeholder="Name" />
                            </FormField>
                            <FormField className={styles.field}>
                                <MySelectInput options={ingredientCategoryOptions} placeholder="Category" name="category" />
                            </FormField>
                        </div>
                        <div className={styles.row}>
                            <FormField className={styles.field}>
                                <MyNumberInput placeholder="Price Per Package" name="pricePerPackage" label="Price Per Package" min={0} step={0.01} />
                            </FormField>
                            <FormField className={styles.field}>
                                <MyNumberInput placeholder="Measurements Per Package" name="measurementsPerPackage" label="Measurements Per Package" min={0} />
                            </FormField>
                        </div>
                        <div className={styles.row}>
                            <FormField className={styles.field}>
                                <MySelectInput options={measuredInOptions} placeholder="Measured In" name="measurementUnit.measuredIn" />
                            </FormField>
                            {values.measurementUnit.measuredIn === MeasuredIn.Weight && (
                                <FormField className={styles.field}>
                                    <MySelectInput placeholder="Weight Unit" name="measurementUnit.weightUnit" options={weightOptions} />
                                </FormField>
                            )}
                            {values.measurementUnit.measuredIn === MeasuredIn.Volume && (
                                <FormField className={styles.field}>
                                    <MySelectInput placeholder="Volume Unit" name="measurementUnit.volumeUnit" options={volumeOptions} />
                                </FormField>
                            )}
                        </div>
                        <div className={styles.row}>
                            <FormField className={styles.field}>
                                <MyNumberInput placeholder="Calories" name="nutrition.calories" label="Calories" min={0} />
                            </FormField>
                            <FormField className={styles.field}>
                                <MyNumberInput placeholder="Protein" name="nutrition.protein" label="Protein" min={0} />
                            </FormField>
                            <FormField className={styles.field}>
                                <MyNumberInput placeholder="Carbs" name="nutrition.carbs" label="Carbs" min={0} />
                            </FormField>
                            <FormField className={styles.field}>
                                <MyNumberInput placeholder="Fat" name="nutrition.fat" label="Fat" min={0} />
                            </FormField>
                        </div>
                        <Button
                            disabled={isSubmitting || !dirty || !isValid}
                            positive
                            type="submit"
                            content="Submit"
                            loading={isSubmitting}
                            floated="right"
                        />
                        <Button as={Link} to="/ingredients" type="button" content="Cancel" floated="right" />
                    </Form>
                )}
            </Formik>
        </Segment>
    );
});



import { Button, Segment, FormField, Header, TextArea } from "semantic-ui-react";
import { Recipe, RecipeCategory } from "../../../app/models/recipe.ts";
import { useStore } from "../../../app/stores/store.ts";
import { observer } from "mobx-react-lite";
import { Link, useNavigate, /*useParams*/ } from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";
import { v4 as uuid } from "uuid";
import { Formik, Form } from "formik";
import MyTextInput from "../../../app/common/form/MyTextInput.tsx";
import MySelectInput from "../../../app/common/form/MySelectInput.tsx";
import { recipeCategoryOptions } from "../../../app/common/options/recipeCategoryOptions.ts";
import MyNumberInput from "../../../app/common/form/MyNumberInput.tsx";
import styles from "./RecipeForm.module.css";

export default observer(function RecipeForm() {
    const { recipeStore, ingredientStore, userStore  } = useStore();
    const { createRecipe, updateRecipe, selectedRecipe, loadingInitial } = recipeStore;
    const { user } = userStore;
    const navigate = useNavigate();

    const initialRecipe: Recipe = {
        id: "",
        name: "",
        recipeCategory: RecipeCategory.Soup,
        servingsPerRecipe: 0,
        instructions: [],
        appUserId: user?.id || "", // Use current user's ID
        totalPrice: 0,
        pricePerServing: 0,
        caloriesPerRecipe: 0,
        carbsPerRecipe: 0,
        fatPerRecipe: 0,
        proteinPerRecipe: 0,
        measurements: []
    };

    const ingredientOptions = ingredientStore.ingredientsByCategory.map((ingredient) => ({
        key: ingredient.id,
        text: ingredient.name,
        value: ingredient.id,
    }));

    const onSubmit = async (values: Recipe, { resetForm: resetForm }) => {
        try {
            if (!values.id) {
                const newId = uuid();
                const newRecipe: Recipe = {
                    ...values,
                    id: newId,
                    appUserId: user?.id || ""
                };
                await createRecipe(newRecipe);
                resetForm();
                navigate(`/recipes/${newRecipe.id}`);
            } else {
                await updateRecipe(values);
                navigate(`/recipes/${values.id}`);
            }
        } catch (error) {
            console.error("Error in onSubmit:", error);
        }
    };

    if (loadingInitial) return <LoadingComponent content="Loading Recipe..." />;

    return (
        <Segment clearing>
            <Header as="h2" textAlign="center" color="teal" className={styles.header}>
                Recipe Form
            </Header>
            <Formik
                enableReinitialize
                initialValues={selectedRecipe || initialRecipe}
                onSubmit={onSubmit}
            >
                {({ values, handleSubmit, isSubmitting, isValid, dirty, setFieldValue }) => (
                    <Form className={`ui form ${styles.form}`} onSubmit={handleSubmit}>
                        {/* Name, Category, and Servings Per Recipe in one row */}
                        <div className={styles.row}>
                            <FormField className={styles.field}>
                                <label>Name</label>
                                <MyTextInput
                                    name="name"
                                    placeholder="Name of Recipe"
                                />
                            </FormField>
                            <FormField className={styles.field}>
                                <label>Category</label>
                                <MySelectInput
                                    options={recipeCategoryOptions}
                                    placeholder="Category"
                                    name="recipeCategory"
                                />
                            </FormField>
                            <FormField className={styles.field}>
                                <label>Servings Per Recipe</label>
                                <MyNumberInput
                                    placeholder="Servings Per Recipe"
                                    name="servingsPerRecipe"
                                    min={1}
                                />
                            </FormField>
                        </div>

                        {/* Full-width Instructions Field (Restored TextArea) */}
                        <div className={styles.fullWidth}>
                            <FormField>
                                <label>Instructions</label>
                                <TextArea
                                    //name="instructionsJson"
                                    placeholder="Enter instructions here..."
                                    value={values.instructions.join('\n')}
                                    onChange={(e) =>
                                        setFieldValue("instructions", e.target.value.split('\n'))
                                    }
                                />
                            </FormField>
                        </div>

                        {/* Ingredient Count Field */}
                        <div className={styles.row}>
                            <FormField className={styles.field}>
                                <label>How many ingredients?</label>
                                <MyNumberInput
                                    placeholder="How many ingredients?"
                                    name="measurements.length"
                                    min={0}
                                    onChange={(e) => {
                                        const count = parseInt(e.target.value, 10) || 0;
                                        setFieldValue("measurements", Array(count).fill({ ingredientId: "", amount: 0 }));
                                    }}
                                />
                            </FormField>
                        </div>

                        {/* Dynamically Generated Ingredient Inputs */}
                        {values.measurements.map((_measurement, index) => (
                            <div key={index} className={styles.row}>
                                <FormField className={styles.field}>
                                    <label>Ingredient {index + 1}</label>
                                    <MySelectInput
                                        options={ingredientOptions}
                                        placeholder="Select Ingredient"
                                        name={`measurements[${index}].ingredientId`}
                                    />
                                </FormField>
                                <FormField className={styles.field}>
                                    <label>Amount</label>
                                    <MyNumberInput
                                        placeholder="Amount"
                                        name={`measurements[${index}].amount`}
                                    />
                                </FormField>
                            </div>
                        ))}

                        <Button
                            disabled={isSubmitting || !dirty || !isValid}
                            positive
                            type="submit"
                            content="Submit"
                            loading={isSubmitting}
                            floated="right"
                        />
                        <Button
                            as={Link}
                            to="/recipes"
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

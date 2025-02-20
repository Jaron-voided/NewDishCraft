import { Button, Segment, FormField, Header } from "semantic-ui-react";
import { DayPlan } from "../../../app/models/dayPlan";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { Link, useNavigate } from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { v4 as uuid } from "uuid";
import { Formik, Form } from "formik";
import MyTextInput from "../../../app/common/form/MyTextInput";
import MySelectInput from "../../../app/common/form/MySelectInput";
import MyNumberInput from "../../../app/common/form/MyNumberInput";
import styles from "./DayPlanForm.module.css";

export default observer(function DayPlanForm() {
    const { dayPlanStore, recipeStore } = useStore();
    const { createDayPlan, updateDayPlan, selectedDayPlan, loadingInitial } = dayPlanStore;
    const navigate = useNavigate();

    const initialDayPlan = selectedDayPlan ?? {
        id: '',
        name: '',
        dayPlanRecipes: [],
        priceForDay: 0,
        caloriesPerDay: 0,
        carbsPerDay: 0,
        fatPerDay: 0,
        proteinPerDay: 0
    };

    // Convert recipes to dropdown options
    const recipeOptions = recipeStore.recipesByCategory.map(recipe => ({
        key: recipe.id,
        text: `${recipe.name}`,
        value: recipe.id
    }));

    const onSubmit = async (values: DayPlan) => {
        try {
            if (!values.id) {
                const newId = uuid();
                const newDayPlan = {
                    ...values,
                    id: newId
                };
                await createDayPlan(newDayPlan, values.dayPlanRecipes.map(dpr => ({
                    recipeId: dpr.recipeId,
                    servings: dpr.servings
                })));
                navigate(`/dayPlan/${newId}`);
            } else {
                await updateDayPlan(values);
                navigate(`/dayPlan/${values.id}`);
            }
        } catch (error) {
            console.error("Error in onSubmit:", error);
        }
    };

    if (loadingInitial) return <LoadingComponent content="Loading Day Plan..." />;

    return (
        <Segment clearing>
            <Header as="h2" textAlign="center" color="teal">
                Day Plan Form
            </Header>
            <Formik
                enableReinitialize
                initialValues={initialDayPlan}
                onSubmit={onSubmit}
            >
                {({ values, handleSubmit, isSubmitting, setFieldValue }) => (
                    <Form className="ui form" onSubmit={handleSubmit}>
                        {/* Name field */}
                        <FormField>
                            <MyTextInput name="name" placeholder="Day Plan Name" />
                        </FormField>

                        {/* Number of recipes selector */}
                        <FormField>
                            <MyNumberInput
                                name="recipeCount"
                                placeholder="How many recipes?"
                                min={1}
                                onChange={(e) => {
                                    const count = parseInt(e.target.value, 10) || 0;
                                    setFieldValue("dayPlanRecipes",
                                        Array(count).fill({
                                            recipeId: "",
                                            servings: 1
                                        })
                                    );
                                }}
                            />
                        </FormField>

                        {/* Dynamic recipe fields */}
                        {values.dayPlanRecipes.map((_recipe, index) => (
                            <div key={index} className={styles.row}>
                                <FormField>
                                    <MySelectInput
                                        options={recipeOptions}
                                        placeholder="Select Recipe"
                                        name={`dayPlanRecipes.${index}.recipeId`}
                                    />
                                </FormField>
                                <FormField>
                                    <MyNumberInput
                                        placeholder="Servings"
                                        name={`dayPlanRecipes.${index}.servings`}
                                        min={1}
                                    />
                                </FormField>
                            </div>
                        ))}

                        <Button
                            positive
                            type="submit"
                            content="Submit"
                            loading={isSubmitting}
                            floated="right"
                        />
                        <Button
                            as={Link}
                            to="/dayPlans"
                            content="Cancel"
                            floated="right"
                        />
                    </Form>
                )}
            </Formik>
        </Segment>
    );
});
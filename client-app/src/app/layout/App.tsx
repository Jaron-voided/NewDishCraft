import {useEffect, useState} from "react";
import axios from "axios";
import {Ingredient, MeasuredIn} from "../models/ingredient.ts";
import NavBar from "./main/NavBar.tsx";
import {Container} from "semantic-ui-react";
import IngredientDashboard from "../../features/ingredients/dashboard/IngredientDashboard.tsx";
import {v4 as uuid} from 'uuid';
import agent from "../api/agent.ts";
import LoadingComponents from "./LoadingComponents.tsx";

function App() {
    const [ingredients, setIngredients] = useState<Ingredient[]>([]);
    const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | undefined>(undefined)
    const [editMode, setEditMode] = useState(false)
    const [loading, setLoading] = useState(true);
    const [submitting, setSubmitting] = useState(false);

    useEffect(() => {
        agent.Ingredients.list()
            .then(response => {
/*                let ingredients: Ingredient[] = [];
                response.forEach(ingredient => {
                    ingredient.date = ingredient.date.split('T')[0]'' +
                    ingredients.push(ingredient);
                })*/
                setIngredients(response);
                setLoading(false);
            })
    }, [])

    function handleSelectIngredient(id: string) {
        setSelectedIngredient(ingredients.find(x => x.id === id))
    }

    function handleCancelSelectIngredient() {
        setSelectedIngredient(undefined)
    }

    function handleFormOpen(id?: string) {
        if (id) {
            handleSelectIngredient(id)
        } else {
            handleCancelSelectIngredient()
        }
        setEditMode(true);
/*
        id ? handleSelectIngredient(id) : handleCancelSelectIngredient()
*/
    }

    function handleFormClose() {
        setEditMode(false);
    }

    function handleCreateOrEditIngredient(ingredient: Ingredient) {
        setSubmitting(true);

        if (!ingredient.id) {
            // Generate a new ID for the ingredient locally
            ingredient.id = uuid();

            // Ensure nutrition is initialized
            if (!ingredient.nutrition) {
                ingredient.nutrition = {
                    ingredientId: ingredient.id,
                    calories: 0,
                    protein: 0,
                    carbs: 0,
                    fat: 0,
                };
            } else {
                ingredient.nutrition.ingredientId = ingredient.id;
            }

            // Ensure measurementUnit is initialized
            if (!ingredient.measurementUnit) {
                ingredient.measurementUnit = {
                    ingredientId: ingredient.id,
                    measuredIn: MeasuredIn.Weight, // Default to Weight
                    weightUnit: undefined,
                    volumeUnit: undefined,
                };
            } else {
                ingredient.measurementUnit.ingredientId = ingredient.id;
            }


            // Create the ingredient first
            agent.Ingredients.create(ingredient)
                .then(() => {
                    // Assign the ID to nutrition and measurementUnit only after the ingredient is created
                    // Update the state with the new ingredient
                    setIngredients([...ingredients, ingredient]);
                    setSelectedIngredient(ingredient);
                    setEditMode(false);
                    setSubmitting(false);
                })
                .catch(error => {
                    console.error("Error creating ingredient:", error);
                    setSubmitting(false);
                });
        } else {
            // For updates
            agent.Ingredients.update(ingredient)
                .then(() => {
                    setIngredients([...ingredients.filter(x => x.id !== ingredient.id), ingredient]);
                    setSelectedIngredient(ingredient);
                    setEditMode(false);
                    setSubmitting(false);
                })
                .catch(error => {
                    console.error("Error updating ingredient:", error);
                    setSubmitting(false);
                });
        }
    }

/*
    function handleCreateOrEditIngredient(ingredient: Ingredient) {
        setSubmitting(true);

        if (!ingredient.id) {
            // Generate a new ID for a new ingredient
            ingredient.id = uuid();

            // Assign the ID to nutrition and measurementUnit
            ingredient.nutrition = {
                ...ingredient.nutrition!,
                ingredientId: ingredient.id,
            };

            ingredient.measurementUnit = {
                ...ingredient.measurementUnit!,
                ingredientId: ingredient.id,
            };

            // Create the new ingredient
            agent.Ingredients.create(ingredient)
                .then(() => {
                    setIngredients([...ingredients, ingredient]);
                    setSelectedIngredient(ingredient);
                    setEditMode(false);
                    setSubmitting(false);
            });
        } else {
            // Update the existing ingredient
            ingredient.nutrition = {
                ...ingredient.nutrition!,
                ingredientId: ingredient.id,
            };

            ingredient.measurementUnit = {
                ...ingredient.measurementUnit!,
                ingredientId: ingredient.id,
            };

            agent.Ingredients.update(ingredient).then(() => {
                setIngredients([...ingredients.filter((x) => x.id !== ingredient.id), ingredient]);
                setSelectedIngredient(ingredient);
                setEditMode(false);
                setSubmitting(false);
            });
        }
    }
*/


    /*   function handleCreateOrEditIngredient(ingredient: Ingredient) {
           setSubmitting(true);

           // Ensure ingredientId is assigned properly
           ingredient.nutrition = {
               ...ingredient.nutrition!,
               ingredientId: ingredient.id // Tie nutrition.ingredientId to ingredient.id
           };

           ingredient.measurementUnit = {
               ...ingredient.measurementUnit!,
               ingredientId: ingredient.id // Tie measurementUnit.ingredientId to ingredient.id
           };

           if (ingredient.id) {
               agent.Ingredients.update(ingredient).then(() => {
                   setIngredients([...ingredients.filter(x => x.id !== ingredient.id), ingredient])
                   setSelectedIngredient(ingredient);
                   setEditMode(false);
                   setSubmitting(false);
               })
           } else {
               ingredient.id = uuid();
               ingredient.nutrition.ingredientId = ingredient.id; // Set ingredientId for nutrition
               ingredient.measurementUnit.ingredientId = ingredient.id; // Set ingredientId for measurementUnit
               agent.Ingredients.create(ingredient).then(() => {
                   setIngredients([...ingredients, ingredient])
                   setSelectedIngredient(ingredient);
                   setEditMode(false);
                   setSubmitting(false);
               })
           }
   /!*        if (ingredient.id) {
               setIngredients([...ingredients.filter(x => x.id !== ingredient.id), ingredient]);
           } else {
               setIngredients([...ingredients, {...ingredient, id: uuid()}]);
           }
           setEditMode(false);
           setSelectedIngredient(ingredient);*!/
       }*/

    function handleDeleteIngredient(id: string) {
        setSubmitting(true);
        agent.Ingredients.delete(id).then (() => {
            setIngredients([...ingredients.filter(x => x.id !== id)])
            setSubmitting(false);
        });
    }

    if (loading) return <LoadingComponents content='Loading...' />

    return (
        <>
            <NavBar
                openForm={handleFormOpen}
            />
            <Container style={{marginTop: "7em"}}>
                <IngredientDashboard
                    ingredients={ingredients}
                    selectedIngredient={selectedIngredient}
                    selectIngredient={handleSelectIngredient}
                    cancelSelectIngredient={handleCancelSelectIngredient}
                    editMode={editMode}
                    openForm={handleFormOpen}
                    closeForm={handleFormClose}
                    createOrEdit={handleCreateOrEditIngredient}
                    deleteIngredient={handleDeleteIngredient}
                    submitting={submitting}
                /> {/*//this takes ingredients from the useState and setIngredients and passes them to IngredientDashboard*/}
            </Container>
        </>

    )
}

export default App


/*
//This was all needed to extract values, but changing the program.cs controllers portion fixes the issue
import {useEffect, useState} from "react";
import axios from "axios";

function App() {
    const [ingredients, setIngredients] = useState([]);
    const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    axios
        .get('http://localhost:5000/api/ingredients')
        .then(response => {
            const data = response.data;
            if (data && data["$values"]) {
                setIngredients(data["$values"]);
            } else {
                setError("Unexpected response format.");
            }
        })
        .catch((err) => {
            console.error(err);
            setError("Failed to fetch ingredients.");
        });
  }, [])

    if (error) {
        return (
            <div className="alert alert-danger">
                <h1>Error</h1>
                <p>{error}</p>
            </div>
        );
    }


    return (
        <div>
            <h1>DishCraft</h1>
            <ul>
                {ingredients.map((ingredients: any) => (
                     <li key={ingredients.id}>
                         {ingredients.name}
                     </li>
                  ))}
             </ul>
         </div>
    );
}

export default App
*/

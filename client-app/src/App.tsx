import './App.css'
import {useEffect, useState} from "react";
import axios from "axios";

function App() {
  const [ingredients, setIngredients] = useState([]);

  useEffect(() => {
    axios.get('http://localhost:5000/api/ingredients')
        .then(response => {
          setIngredients(response.data);
        })
  }, [])


  return (
      <div>
          <h1>DishCraft</h1>
          <ul>
              {ingredients.map((ingredient: any) => (
                  <li key={ingredient.id}>
                      {ingredient.name}
                  </li>
              ))}
          </ul>
      </div>

  )
}

export default App

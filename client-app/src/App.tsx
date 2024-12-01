import './App.css'
import {useEffect, useState} from "react";
import axios from "axios";

function App() {
  const [ingredients, setIngredients] = useState([]);

  useEffect(() => {
    axios.get('/api/ingredients')
        .then(response => {
          setIngredients(response.data);
        })
  }, [])


  return (
      <h1>DishCraft</h1>
  )
}

export default App

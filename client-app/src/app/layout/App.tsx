import {useEffect} from "react";
import NavBar from "./main/NavBar.tsx";
import {Container} from "semantic-ui-react";
import IngredientDashboard from "../../features/ingredients/dashboard/IngredientDashboard.tsx";;
import LoadingComponent from "./LoadingComponent.tsx";
import {useStore} from "../stores/store.ts";
import {observer} from "mobx-react-lite";

function App() {
    const {ingredientStore} = useStore();

    useEffect(() => {
        if (ingredientStore.ingredientsByPrice.length === 0) {
            ingredientStore.loadIngredients();
        }
    }, [ingredientStore]);

    if (ingredientStore.loadingInitial) return <LoadingComponent content='Loading...' />

    return (
        <>
            <NavBar />
            <Container style={{marginTop: "7em"}}>
                <IngredientDashboard />
            </Container>
        </>

    )
}

export default observer(App);


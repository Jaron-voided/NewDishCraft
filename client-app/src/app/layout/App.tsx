import NavBar from "./main/NavBar.tsx";
import {Container} from "semantic-ui-react";
import {observer} from "mobx-react-lite";
import {Outlet, useLocation} from "react-router-dom";
import HomePage from "../../features/home/HomePage.tsx";

function App() {
    const location = useLocation();

    return (
        <>
            {location.pathname === "/" ? <HomePage /> : (
                <>
                    <NavBar />
                    <Container style={{marginTop: "7em"}}>
                        <Outlet />
                    </Container>
                </>
            )}

        </>

    )
}

export default observer(App);


import {Button, Container, Header, Segment, Image} from "semantic-ui-react";
import {Link} from "react-router-dom";
import NavBar from "../../app/layout/main/NavBar.tsx";
import {observer} from "mobx-react-lite";
import {useStore} from "../../app/stores/store.ts";
import LoginForm from "../users/LoginForm.tsx";
import RegisterForm from "../users/RegisterForm.tsx";

export default observer(function HomePage() {
    const {userStore, modalStore} = useStore();
    return (
        <Segment inverted textAlign="center" vertical className="masthead">
            {userStore.isLoggedIn ? (
                <>
                    <NavBar />
                </>
            ) : (
                <>
                    <Button onClick={() => modalStore.openModal(<LoginForm />)} size='huge' inverted>
                        Login!
                    </Button>
                    <Button onClick={() => modalStore.openModal(<h1>Register</h1>)} size='huge' inverted>
                        Register!
                    </Button>
                </>
            )}
            <Container text>
                {/* Welcome Header */}
                <Header as="h1" className="welcome-header" inverted style={{ marginBottom: '2em', marginTop: '0' }}>
                    Welcome to DishCraft
                </Header>


                {/* Logo Image */}
                <Image
                    className="header-image"
                    src="/assets/bigLogo.jpg"
                    alt="logo"
                    centered
                    size="big"
                    style={{ marginTop: '2em' }}
                />
                <Button onClick={() => modalStore.openModal(<LoginForm />)} size='huge' inverted>
                    Login!
                </Button>
                <Button onClick={() => modalStore.openModal(<RegisterForm />)} size='huge' inverted>
                    Register!
                </Button>
  {/*              <Button
                    as={Link}
                    to='/login'
                    size='huge'
                    inverted
                >
                    Login!
                </Button>*/}

      {/*           Title Below the Image
                <Header as="h1" className="header-title" inverted>
                    DishCraft
                </Header>*/}
{/*
                 Navigation Button
                <Button as={Link} to="/ingredients" size="huge" inverted>
                    Go to Ingredients
                </Button>

                <Button as={Link} to="/recipes" size="huge" inverted>
                    Go to Recipes
                </Button>*/}
            </Container>
        </Segment>
    );
})

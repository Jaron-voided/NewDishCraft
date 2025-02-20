import {Button, Container, Header, Segment, Image} from "semantic-ui-react";
import NavBar from "../../app/layout/main/NavBar/NavBar.tsx";
import {observer} from "mobx-react-lite";
import {useStore} from "../../app/stores/store.ts";
import LoginForm from "../users/LoginForm.tsx";
import RegisterForm from "../users/RegisterForm.tsx";

export default observer(function HomePage() {
    const {userStore, modalStore} = useStore();

    return (
        <Segment
            inverted
            textAlign="center"
            vertical
            className="masthead"
            style={{
                minHeight: '100vh', // Ensure full screen height
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center', // Center content vertically
                alignItems: 'center', // Center content horizontally
                padding: '2em',
            }}
        >
            <Container text>
                {/* Logo Image */}
                <Image
                    className="header-image"
                    src="/assets/bigLogo.jpg"
                    alt="logo"
                    centered
                    size="big"
                    style={{
                        marginBottom: '1em',
                        marginTop: '-1em',
                    }}
                />

                {/* Conditional Rendering for User State */}
                {userStore.isLoggedIn ? (
                    <>
                        {/* NavBar and Personalized Welcome */}
                        <NavBar />
                        <Header
                            as="h1"
                            className="welcome-header"
                            inverted
                            style={{
                                marginBottom: '0.5em',
                                fontSize: '2.5em',
                                fontWeight: 'bold',
                                marginTop: '-0.5em',
                            }}
                        >
                            Welcome back, {userStore.user?.displayName}!
                        </Header>
                    </>
                ) : (
                    <>
                        {/* Welcome Header */}
                        <Header
                            as="h1"
                            className="welcome-header"
                            inverted
                            style={{
                                marginBottom: '0.5em',
                                fontSize: '2.5em',
                                fontWeight: 'bold',
                                marginTop: '-0.5em',
                            }}
                        >
                            Welcome to DishCraft
                        </Header>

                        {/* Login/Register Buttons */}
                        <div
                            style={{
                                display: 'flex',
                                justifyContent: 'center',
                                gap: '1.5em',
                                marginTop: '1em',
                            }}
                        >
                            <Button
                                onClick={() => modalStore.openModal(<LoginForm />)}
                                size="huge"
                                inverted
                            >
                                Login!
                            </Button>
                            <Button
                                onClick={() => modalStore.openModal(<RegisterForm />)}
                                size="huge"
                                inverted
                            >
                                Register!
                            </Button>
                        </div>
                    </>
                )}
            </Container>
        </Segment>
    );
});


/*


export default observer(function HomePage() {
    const {userStore, modalStore} = useStore();
    return (
        <Segment
            inverted
            textAlign="center"
            vertical
            className="masthead"
            style={{
                    minHeight: '100vh', // Ensure full screen height
                    paddingTop: '8em', // Add padding to account for the fixed NavBar
                    paddingBottom: '2em', // Add padding at the bottom
                    display: 'flex',
                    flexDirection: 'column',
                    justifyContent: 'center', // Center content vertically
                    alignItems: 'center', // Center content horizontally
            }}
        >
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
                {/!* Welcome Header *!/}
                <Header
                    as="h1"
                    className="welcome-header"
                    inverted
                    style={{
                        marginBottom: '0.5em',
                        fontSize: '2.5em',
                        fontWeight: 'bold',
                        marginTop: '-0.5em'
                    }}>
                    Welcome to DishCraft
                </Header>


                {/!* Logo Image *!/}
                <Image
                    className="header-image"
                    src="/assets/bigLogo.jpg"
                    alt="logo"
                    centered
                    size="big"
                    style={{
                        marginBottom: '1em',
                        marginTop: '-1em'
                }}
                />
                <div style={{ display: 'flex', justifyContent: 'center', gap: '1.5em', marginTop: '1em' }}>
                    <Button onClick={() => modalStore.openModal(<LoginForm />)} size="huge" inverted>
                        Login!
                    </Button>
                    <Button onClick={() => modalStore.openModal(<RegisterForm />)} size="huge" inverted>
                        Register!
                    </Button>
                </div>
            </Container>
        </Segment>
    );
})
*/

import {Button, Container, Header, Segment, Image} from "semantic-ui-react";
import {Link} from "react-router-dom";
import NavBar from "../../app/layout/main/NavBar.tsx";

export default function HomePage() {
    return (
        <Segment inverted textAlign="center" vertical className="masthead">
            <NavBar />
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
}

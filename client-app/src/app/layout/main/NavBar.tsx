import {Button, Container, Menu} from "semantic-ui-react";
import {NavLink} from "react-router-dom";




export default function NavBar() {

    return (
        <Menu inverted fixed='top'>
            <Container>
                <Menu.Item as={NavLink} to='/' header>
                    <img src={"/assets/logo.png"} alt="logo" style={{marginRight: '10px'}}/>
                    DishCraft
                </Menu.Item>
                <Menu.Item as={NavLink} to='/ingredients' name='Ingredients' />
                <Menu.Item>
                    <Button
                        positive
                        content='Create Ingredient'
                        as={NavLink} to='/createIngredient'
                    />
                </Menu.Item>
            </Container>
        </Menu>
    )
}
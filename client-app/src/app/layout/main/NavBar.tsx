import {Button, Container, Menu} from "semantic-ui-react";
import {useStore} from "../../stores/store.ts";




export default function NavBar() {

    const {ingredientStore} = useStore();

    return (
        <Menu inverted fixed='top'>
            <Container>
                <Menu.Item header>
                    <img src={"/assets/logo.png"} alt="logo" style={{marginRight: '10px'}}/>
                    DishCraft
                </Menu.Item>
                <Menu.Item name='Ingredients' />
                <Menu.Item>
                    <Button
                        positive
                        content='Create Ingredient'
                        onClick={() => ingredientStore.openForm()}
                    />
                </Menu.Item>
            </Container>
        </Menu>
    )
}
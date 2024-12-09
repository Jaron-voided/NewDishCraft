import {Button, Container, Menu} from "semantic-ui-react";

interface Props {
    openForm: () => void;
}


export default function NavBar({openForm}: Props) {
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
                        onClick={openForm}
                    /> {/*//positive makes it green*/}
                </Menu.Item>
            </Container>
        </Menu>
    )
}
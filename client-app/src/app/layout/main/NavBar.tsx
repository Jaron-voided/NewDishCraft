import {Button, Container, Dropdown, Image, Menu} from "semantic-ui-react";
import {Link, NavLink} from "react-router-dom";
import IngredientFilters from "../../../features/ingredients/dashboard/IngredientFilters.tsx";
import {useStore} from "../../stores/store.ts";
import {observer} from "mobx-react-lite";




export default observer(function NavBar() {
    const {userStore : {user, logout}} = useStore();

    return (
        <Menu inverted fixed="top">
            <Container
                style={{
                    position: "relative", // Enables absolute positioning for the left content
                    display: "flex",
                    alignItems: "center",
                    //justifyContent: "center", // Centers the middle content
                    width: "100%",
                }}
            >
                {/* Left-aligned logo and text */}
                <Menu.Item
                    as={NavLink}
                    to="/"
                    header
                    style={{
                        //position: "absolute", // Keeps this content fixed to the left
                        left: "10px",
                        display: "flex",
                        alignItems: "left",
                        margin: 0,
                    }}
                >
                    <Image
                        src={"/assets/logo.png"}
                        size="tiny"
                        alt="logo"
                        style={{ marginRight: "10px" }}
                    />
                    <span style={{ fontSize: "2.5em", fontWeight: "bold" }}>Dish Craft</span>
                </Menu.Item>

                {/* Centered menu items */}
                <div
                    style={{
                        position: "relative",
                        display: "flex",
                        alignItems: "center",
                        gap: "20px",
                    }}
                >
                    <Button
                        color="teal"
                        size="big"
                        content="Ingredients"
                        as={NavLink}
                        to="/ingredients"
                    />
                    <Button
                        color="teal"
                        size="big"
                        content="Recipes"
                        as={NavLink}
                        to="/recipes"
                    />
                    <Button
                        primary
                        size="big"
                        content="Create Ingredient"
                        as={NavLink}
                        to="/createIngredient"
                    />
                    <Button
                        primary
                        size="big"
                        content="Create Recipe"
                        as={NavLink}
                        to="/createRecipe"
                    />
                </div>
                <Menu.Item position='right'>
                    <Image src={user?.image || '/assets/logo.png'} avatar spaced='right' />
                    <Dropdown pointing='top left' text={user?.displayName}>
                        <Dropdown.Menu>
                            <Dropdown.Item as={Link} to={`/profile/${user?.username}`} text='My Profile' icon='user'/>
                            <Dropdown.Item onClick={logout} text='Logout' icon='power'/>
                        </Dropdown.Menu>
                    </Dropdown>
                </Menu.Item>
            </Container>
        </Menu>
    );


 /*   return (
        <Menu inverted fixed='top' >
            <Container
                style={{
                    display: 'flex',
                    alignItems: 'center',
                    width: '100%',
                }}
            >
                {/!* Left-aligned logo and text *!/}
                <div
                    style={{
                        display: "flex",
                        alignItems: "center",
                        gap: "10px",
                        left: 0,
                        paddingLeft: "10px",
                    }}
                >
                    <Menu.Item
                        as={NavLink}
                        to="/"
                        header
                        style={{ display: "flex", alignItems: "center", margin: 0}}
                    >
                        <Image
                            src={"/assets/logo.png"}
                            size="tiny"
                            alt="logo"
                            style={{ marginRight: "10px" }}
                        />
                        <span style={{ fontSize: "2.5em", fontWeight: "bold" }}>Dish Craft</span>
                    </Menu.Item>
                </div>

                {/!* Centered menu items *!/}
                <div style={{ display: "flex", alignItems: "center", gap: "20px", }}>
                    <Menu.Item as={NavLink} to="/ingredients" name="Ingredients" />
                    <Menu.Item as={NavLink} to="/recipes" name="Recipes" />
                    <Button
                        positive
                        content="Create Ingredient"
                        as={NavLink}
                        to="/createIngredient"
                    />
                    <Button
                        positive
                        content="Create Recipe"
                        as={NavLink}
                        to="/createRecipe"
                    />
                </div>


            </Container>
        </Menu>
    )
 */
})
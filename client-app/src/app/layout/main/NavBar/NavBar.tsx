import { Dropdown, Image, Menu } from "semantic-ui-react";
import { Link, NavLink } from "react-router-dom";
import { useStore } from "../../../stores/store.ts";
import { observer } from "mobx-react-lite";
import styles from "./NavBar.module.css";

export default observer(function NavBar() {
    const {
        userStore: { user, logout }, recipeStore
    } = useStore();


    const navItems = [
        {
            name: 'Ingredients',
            color: 'teal',
            className: styles.ingredientsDropdown,
            items: [
                { name: 'View Ingredients', path: '/ingredients', icon: 'eye' },
                { name: 'Add Ingredient', path: '/createIngredient', icon: 'plus' }
            ]
        },
        {
            name: 'Recipes',
            color: 'google plus',
            className: styles.recipesDropdown,
            items: [
                { name: 'View Recipes', path: '/recipes', icon: 'eye' },
                { name: 'Add Recipe', path: '/createRecipe', icon: 'plus' }
            ]
        },
        {
            name: 'DayPlans',
            color: 'pink',
            className: styles.dayPlansDropdown,
            items: [
                { name: 'View DayPlans', path: '/dayPlan', icon: 'eye' },
                { name: 'Add DayPlan', path: '/createDayPlan', icon: 'plus' }
            ]
        }
    ];

    return (
        <Menu inverted fixed="top" className={styles.navBar}>
            <div className={styles.container}>
                {/* Left-aligned logo and text */}
                <Menu.Item as={NavLink} to="/" header className={styles.logoSection}>
                    <Image src={"/assets/logo.png"} alt="logo" className={styles.logo} />
                    <span className={styles.logoText}>Dish Craft</span>
                </Menu.Item>

                {/* Centered dropdown menus */}
                <div className={styles.navButtonsContainer}>
                    {navItems.map((item) => (
                        <Dropdown
                            key={item.name}
                            text={item.name}
                            className={`${styles.navDropdown} ${item.className} ui button ${item.color}`}
                        >
                            <Dropdown.Menu>
                                {item.items.map((subItem) => (
                                    <Dropdown.Item
                                        key={subItem.name}
                                        as={NavLink}
                                        to={subItem.path}
                                        onClick={() => {
                                            if (subItem.name === 'Add Recipe') {
                                                recipeStore.clearSelectedRecipe();
                                            }
                                        }}
                                        icon={subItem.icon}
                                        text={subItem.name}
                                    />
                                ))}
                            </Dropdown.Menu>
                        </Dropdown>
                    ))}
                </div>

                {/* User dropdown menu */}
                <Menu.Item position="right" className={styles.userSection}>
                    <Dropdown
                        pointing="top right"
                        className={styles.userDropdown}
                        trigger={
                            <span>
                                <Image src={user?.image || "/assets/logo.png"} avatar className={styles.avatar} />
                                {user?.displayName}
                            </span>
                        }
                    >
                        <Dropdown.Menu>
                            <Dropdown.Item as={Link} to={`/profile/${user?.username}`} text="My Profile" icon="user" />
                            <Dropdown.Item onClick={logout} text="Logout" icon="power" />
                        </Dropdown.Menu>
                    </Dropdown>
                </Menu.Item>
            </div>
        </Menu>
    );
});
/*
import { Button, Dropdown, Image, Menu } from "semantic-ui-react";
import { Link, NavLink } from "react-router-dom";
import { useStore } from "../../../stores/store.ts";
import { observer } from "mobx-react-lite";
import styles from "./NavBar.module.css";

export default observer(function NavBar() {
    const {
        userStore: { user, logout },
    } = useStore();

    return (
        <Menu inverted fixed="top" className={styles.navBar}>
            <div className={styles.container}>
                {/!* Left-aligned logo and text *!/}
                <Menu.Item as={NavLink} to="/" header className={styles.logoSection}>
                    <Image src={"/assets/logo.png"} alt="logo" className={styles.logo} />
                    <span className={styles.logoText}>Dish Craft</span>
                </Menu.Item>

                {/!* Centered menu items *!/}
                <div className={styles.navButtonsContainer}>
                    <Button
                        color="teal"
                        size="big"
                        content="View Ingredients"
                        as={NavLink}
                        to="/ingredients"
                        icon="eye"
                        labelPosition="left"
                        className={styles.navButton}
                    />
                    <Button
                        color="teal"
                        size="big"
                        content="Add Ingredient"
                        as={NavLink}
                        to="/createIngredient"
                        icon="plus"
                        labelPosition="left"
                        className={styles.navButton}
                    />
                    <Button
                        color="pink"
                        size="big"
                        content="View DayPlans"
                        as={NavLink}
                        to="/dayPlan"
                        icon="eye"
                        labelPosition="left"
                        className={styles.navButton}
                    />
                    <Button
                        color="pink"
                        size="big"
                        content="Add DayPlan"
                        as={NavLink}
                        to="/createDayPlan"
                        icon="plus"
                        labelPosition="left"
                        className={styles.navButton}
                    />
                    <Button
                        color="google plus"
                        size="big"
                        content="View Recipes"
                        as={NavLink}
                        to="/recipes"
                        icon="eye"
                        labelPosition="left"
                        className={styles.navButton}
                    />
                    <Button
                        color="google plus"
                        size="big"
                        content="Add Recipe"
                        as={NavLink}
                        to="/createRecipe"
                        icon="plus"
                        labelPosition="left"
                        className={styles.navButton}
                    />
                    <Button
                        color="grey"
                        size="big"
                        content="Me"
                        as={Link}
                        to={`/profile/${user?.username}`}
                        icon="user"
                        labelPosition="left"
                        className={styles.navButton}
                    />
                </div>

                {/!* User dropdown menu *!/}
                <Menu.Item position="right" className={styles.userSection}>
                    <Image
                        src={user?.image || "/assets/logo.png"}
                        avatar
                        className={styles.avatar}
                    />
                    <Dropdown pointing="top right" text={user?.displayName} className={styles.userDropdown}>
                        <Dropdown.Menu>
                            <Dropdown.Item onClick={logout} text="Logout" icon="power" />
                        </Dropdown.Menu>
                    </Dropdown>
                </Menu.Item>
            </div>
        </Menu>
    );
});
*/

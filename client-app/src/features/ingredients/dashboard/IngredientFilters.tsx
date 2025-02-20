import { Button, Dropdown, Header, Input, Menu } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store.ts";
import { observer } from "mobx-react-lite";
import { IngredientCategory } from "../../../app/models/ingredient.ts";
import styles from "./IngredientFilter.module.css";

export default observer(function IngredientFilters() {
    const { ingredientStore: { predicate, setPredicate } } = useStore();

    // Convert IngredientCategory enum into dropdown options
    const categoryOptions = Object.values(IngredientCategory).map((category) => ({
        key: category,
        text: category,
        value: category,
    }));

    return (
        <Menu vertical fluid size="large" className={styles.filterMenu}>
            <Header icon="filter" attached color="teal" content="Filters" />

            {/* Row for "All Ingredients", "Price High", and "Price Low" */}
            <Menu.Item>
                <div className={styles.filterButtonsRow}>
                    <Button
                        content="All Ingredients"
                        color="teal"
                        size="small"
                        active={predicate.has("all")}
                        onClick={() => setPredicate("all", "true")}
                        className={styles.filterButton}
                    />
                    <Button
                        content="Price High"
                        color="teal"
                        size="small"
                        active={predicate.has("priceHigh")}
                        onClick={() => setPredicate("priceHigh", "true")}
                        className={styles.filterButton}
                    />
                    <Button
                        content="Price Low"
                        color="teal"
                        size="small"
                        active={predicate.has("priceLow")}
                        onClick={() => setPredicate("priceLow", "true")}
                        className={styles.filterButton}
                    />
                </div>
            </Menu.Item>

            {/* Dropdown for categories */}
            <Menu.Item>
                <label className={styles.filterLabel}>By Category</label>
                <Dropdown
                    placeholder="Select Category"
                    fluid
                    selection
                    options={categoryOptions}
                    value={predicate.get("category") || ""}
                    onChange={(_e, data) => setPredicate("category", data.value as IngredientCategory)}
                    className={styles.dropdown}
                />
            </Menu.Item>

            {/* Search input */}
            <Menu.Item>
                <Input
                    placeholder="Search Ingredients..."
                    fluid
                    className={styles.searchInput}
                    onChange={(e) => setPredicate("search", e.target.value)}
                />
            </Menu.Item>
        </Menu>
    );
});


/*
import { Button, Dropdown, Header, Input, Menu } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store.ts";
import { observer } from "mobx-react-lite";
import { IngredientCategory } from "../../../app/models/ingredient.ts";

export default observer(function IngredientFilters() {
    const { ingredientStore: { predicate, setPredicate } } = useStore();

    // Convert IngredientCategory enum into dropdown options
    const categoryOptions = Object.values(IngredientCategory).map((category) => ({
        key: category,
        text: category,
        value: category,
    }));

    return (
        <Menu vertical fluid size="large" style={{ width: "100%", marginTop: "1rem" }}>
            <Header icon="filter" attached color="teal" content="Filters" />

            {/!* Centered "All Ingredients" *!/}
            <Menu.Item>
                <Header
                    as="h4"
                    style={{ textAlign: "center", margin: "0.5rem 0", color: "teal" }}
                >
                    All Ingredients
                </Header>
            </Menu.Item>

            {/!* "Price High" and "Price Low" Buttons *!/}
            <Menu.Item>
                <div style={{ display: "flex", justifyContent: "space-between", gap: "1rem" }}>
                    <Button
                        content="Price High"
                        color="teal"
                        size="small"
                        active={predicate.has("priceHigh")}
                        onClick={() => setPredicate("priceHigh", "true")}
                    />
                    <Button
                        content="Price Low"
                        color="teal"
                        size="small"
                        active={predicate.has("priceLow")}
                        onClick={() => setPredicate("priceLow", "true")}
                    />
                </div>
            </Menu.Item>

            {/!* "By Category" Dropdown *!/}
            <Menu.Item>
                <label style={{ marginBottom: "0.5rem", display: "block" }}>By Category</label>
                <Dropdown
                    placeholder="Select Category"
                    fluid
                    selection
                    options={categoryOptions}
                    value={predicate.get("category") || ""}
                    onChange={(_e, data) => setPredicate("category", data.value as IngredientCategory)}
                />
            </Menu.Item>

            {/!* Search Bar *!/}
            <Menu.Item>
                <label style={{ marginBottom: "0.5rem", display: "block" }}>Search</label>
                <Input
                    icon="search"
                    placeholder="Search ingredients..."
                    fluid
                    onChange={(e) => setPredicate("search", e.target.value)}
                />
            </Menu.Item>
        </Menu>
    );
});

*/

/*
import {Dropdown, Header, Menu} from "semantic-ui-react";
import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";
import {IngredientCategory} from "../../../app/models/ingredient.ts";


export default observer(function IngredientFilters() {
    const {ingredientStore: {predicate, setPredicate}} = useStore();

    // Convert IngredientCategory enum into dropdown options
    const categoryOptions = Object.values(IngredientCategory).map((category) => ({
        key: category,
        text: category,
        value: category,
    }));

    return (
        <>
            <Menu vertical size='large' style={{width: '100%', marginTop: 25}}>
                <Header icon='filter' attached color='teal' content='filters' />
                <Menu.Item
                    content='All Ingredients'
                    active={predicate.has('all')}
                    onClick={() => setPredicate('all', 'true')}
                />
                <Menu.Item
                    content='Price High'
                    active={predicate.has('priceHigh')}
                    onClick={() => setPredicate('priceHigh', 'true')}
                />
                <Menu.Item
                    content='Price Low'
                    active={predicate.has('priceLow')}
                    onClick={() => setPredicate('priceLow', 'true')}
                />
                <Menu.Item>
                    <label>By Category</label>
                    <Dropdown
                        placeholder='Select Category'
                        fluid
                        selection
                        options={categoryOptions}
                        value={predicate.get('category') || ''}
                        onChange={(_e, data) => setPredicate('category', data.value as IngredientCategory)}
                    />
                </Menu.Item>
            </Menu>
        </>
    )
})*/

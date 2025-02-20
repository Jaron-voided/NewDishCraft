import {Button, Dropdown, Header, Input, Menu} from "semantic-ui-react";
import styles from "../../ingredients/dashboard/IngredientFilter.module.css";
import {useStore} from "../../../app/stores/store.ts";
import {RecipeCategory} from "../../../app/models/recipe.ts";
import {observer} from "mobx-react-lite";


export default observer(function RecipeFilters() {
    const {recipeStore: {predicate, setPredicate}} = useStore();

    // Convert IngredientCategory enum into dropdown options
    const categoryOptions = Object.values(RecipeCategory).map((category) => ({
        key: category,
        text: category,
        value: category,
    }));

    return (
        <Menu vertical fluid size="large" className={styles.filterMenu}>
            <Header icon="filter" attached color="teal" content="Filters"/>

            {/* Row for "All Recipes", "Price High", and "Price Low" */}
            <Menu.Item>
                <div className={styles.filterButtonsRow}>
                    <Button
                        content="All Recipes"
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
                    onChange={(_e, data) => setPredicate("category", data.value as RecipeCategory)}
                    className={styles.dropdown}
                />
            </Menu.Item>

            {/* Search input */}
            <Menu.Item>
                <Input
                    placeholder="Search Recipes..."
                    fluid
                    className={styles.searchInput}
                    onChange={(e) => setPredicate("search", e.target.value)}
                />
            </Menu.Item>
        </Menu>
    );
});

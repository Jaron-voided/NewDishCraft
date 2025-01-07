import {Header, Menu} from "semantic-ui-react";
import {Calendar} from "react-calendar";


export default function RecipeFilters() {
    return (
        <>
            <Menu vertical size='large' style={{width: '100%', marginTop: 25}}>
                <Header icon='filter' attached color='teal' content='filters' />
                <Menu.Item content='All Recipes' />
            </Menu>
            <Header />
            <Calendar />
        </>
    )
}
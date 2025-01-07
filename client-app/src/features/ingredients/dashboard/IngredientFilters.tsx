import {Header, Menu} from "semantic-ui-react";
import {Calendar} from "react-calendar";


export default function IngredientFilters() {
    return (
        <>
            <Menu vertical size='large' style={{width: '100%', marginTop: 25}}>
                <Header icon='filter' attached color='teal' content='filters' />
                <Menu.Item content='All Ingredients' />
            </Menu>
            <Header />
            <Calendar />
        </>
    )
}
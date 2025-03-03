import {Button, Header, Icon, Segment} from "semantic-ui-react";
import {Link} from "react-router-dom";

export default function NotFound(){
    return (
        <Segment placeholder>
            <Header icon>
                <Icon name='search' />
                Oops - we've looked everywhere but could not find what you are looking for!
            </Header>
            <Segment.Inline>
                <Button as ={Link} to='/'>
                    Return to the Home Page
                </Button>
            </Segment.Inline>
        </Segment>
    )
}

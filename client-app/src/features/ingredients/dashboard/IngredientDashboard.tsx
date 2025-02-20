import {Grid, Icon, Loader, Segment} from "semantic-ui-react";
import IngredientList from "./IngredientList.tsx";

import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";
import {useEffect, useState} from "react";
import IngredientFilters from "./IngredientFilters.tsx";
import {PagingParams} from "../../../app/models/pagination.ts";
import InfiniteScroll from "react-infinite-scroller";
import IngredientListItemPlaceholder from "./IngredientListItemPlaceholder.tsx";


export default observer(function IngredientDashboard() {

    const {ingredientStore} = useStore();
    const {loadIngredients, ingredientRegistry, setPagingParams, pagination} = ingredientStore;
    const [loadingNext, setLoadingNext] = useState(false);

    function handleGetNext() {
        setLoadingNext(true);
        setPagingParams(new PagingParams(pagination!.currentPage + 1));
        loadIngredients().then(() => setLoadingNext(false));
    }

    useEffect(() => {
        if (ingredientRegistry.size <= 1) loadIngredients();
    }, [loadIngredients, ingredientRegistry.size]);


    //if (ingredientStore.loadingInitial && !loadingNext) return <LoadingComponent content='Loading ingredients...' />

    return (
        <Grid centered>
            <Grid.Column width={10}>
                {ingredientStore.loadingInitial && ingredientRegistry.size === 0 && !loadingNext ? (
                    <>
                        <IngredientListItemPlaceholder />
                        <IngredientListItemPlaceholder />
                    </>
                ) : ingredientRegistry.size === 0 ? (
                    <Segment placeholder textAlign="center">
                        <Icon name='book' size='large' />
                        <h3>You currently have no ingredients</h3>
                        <p>You need to add ingredients in your Pantry</p>
                        <p>To add a ingredient, click the 'Add Ingredient' button</p>
                    </Segment>
                ) : (
                    <Segment textAlign="center">
                        <InfiniteScroll
                            pageStart={0}
                            loadMore={handleGetNext}
                            hasMore={!loadingNext && !!pagination && pagination.currentPage < pagination.totalPages}
                            initialLoad={false}
                        >
                            <IngredientList />
                        </InfiniteScroll>
                    </Segment>
                )}

            </Grid.Column>
            <Grid.Column width={6}>
                <IngredientFilters />
            </Grid.Column>
            <Grid.Column width={10}>
                <Loader active={loadingNext} />
            </Grid.Column>
        </Grid>
    );
})
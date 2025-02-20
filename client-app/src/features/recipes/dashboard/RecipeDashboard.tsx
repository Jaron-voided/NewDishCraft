import {observer} from "mobx-react-lite";
import {useStore} from "../../../app/stores/store.ts";
import {useEffect, useState} from "react";
import {Grid, Icon, Loader, Segment} from "semantic-ui-react";
import RecipeFilters from "./RecipeFilters.tsx";
import RecipeList from "./RecipeList.tsx";
import {PagingParams} from "../../../app/models/pagination.ts";
import InfiniteScroll from "react-infinite-scroller";
import RecipeListItemPlaceholder from "./RecipeListItemPlaceholder.tsx";


export default observer(function RecipeDashboard() {
    const {recipeStore} = useStore();
    const {loadRecipes, recipeRegistry, setPagingParams, pagination} = recipeStore;
    const [loadingNext, setLoadingNext] = useState(false);

    function handleGetNext() {
        setLoadingNext(true);
        setPagingParams(new PagingParams(pagination!.currentPage + 1));
        loadRecipes().then(() => setLoadingNext(false));
    }


    useEffect(() => {
        if (recipeRegistry.size <= 1) loadRecipes();

    }, [loadRecipes, recipeRegistry.size]);

    //if (recipeStore.loadingInitial && !loadingNext) return <LoadingComponent content='Loading...' />

    return (
        <Grid>
            <Grid.Column width='10'>
                {recipeStore.loadingInitial && recipeRegistry.size === 0 && !loadingNext ? (
                    <>
                        <RecipeListItemPlaceholder />
                        <RecipeListItemPlaceholder />
                    </>
                ) : recipeRegistry.size === 0 ? (
                    <Segment placeholder textAlign="center">
                        <Icon name='book' size='large' />
                        <h3>You currently have no recipes</h3>
                        <p>First, make sure you have ingredients in your Pantry</p>
                        <p>Then to add a recipe, click the 'Add Recipe' button</p>
                    </Segment>
                ) : (
                    <Segment textAlign="center">
                        <InfiniteScroll
                            pageStart={0}
                            loadMore={handleGetNext}
                            hasMore={!loadingNext && !!pagination && pagination.currentPage < pagination.totalPages}
                            initialLoad={false}
                        >
                            <RecipeList />
                        </InfiniteScroll>
                    </Segment>
                )}

            </Grid.Column>
            <Grid.Column width='6'>
                <RecipeFilters />
            </Grid.Column>
            <Grid.Column width={10}>
                <Loader active={loadingNext} />
            </Grid.Column>
        </Grid>
    )
})

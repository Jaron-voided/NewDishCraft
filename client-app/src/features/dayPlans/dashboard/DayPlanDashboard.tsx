import {observer} from "mobx-react-lite";
import {useStore} from "../../../app/stores/store.ts";
import {useEffect, useState} from "react";
import {Grid, Icon, Loader, Segment} from "semantic-ui-react";
import {PagingParams} from "../../../app/models/pagination.ts";
import InfiniteScroll from "react-infinite-scroller";
import DayPlanListItemPlaceholder from "./DayPlanListItemPlaceholder.tsx";
import DayPlanList from "./DayPlanList.tsx";
//import DayPlanFilters from "./DayPlanFilters.tsx";


export default observer(function DayPlanDashboard() {
    const {dayPlanStore} = useStore();
    //const {recipeStore} = useStore();
    const {loadDayPlans, dayPlanRegistry, setPagingParams, pagination} = dayPlanStore;
    const [loadingNext, setLoadingNext] = useState(false);

    function handleGetNext() {
        setLoadingNext(true);
        setPagingParams(new PagingParams(pagination!.currentPage + 1));
        loadDayPlans().then(() => setLoadingNext(false));
    }


    useEffect(() => {
        if (dayPlanRegistry.size <= 1) loadDayPlans();

    }, [loadDayPlans, dayPlanRegistry.size]);

    //if (dayPlanStore.loadingInitial && !loadingNext) return <LoadingComponent content='Loading...' />

    return (
        <Grid>
            <Grid.Column width='10'>
                {dayPlanStore.loadingInitial && dayPlanRegistry.size === 0 && !loadingNext ? (
                    <>
                        <DayPlanListItemPlaceholder />
                        <DayPlanListItemPlaceholder />
                    </>
                ) : dayPlanRegistry.size === 0 ? (
                    <Segment placeholder textAlign="center">
                        <Icon name='book' size='large' />
                        <h3>You currently have no dayPlans</h3>
                        <p>First, make sure you have ingredients in your Pantry</p>
                        <p>Then, create recipes in your Pantry</p>
                        <p>Then to add a dayPlan, click the 'Add DayPlan' button</p>
                    </Segment>
                ) : (
                    <Segment textAlign="center">
                        <InfiniteScroll
                            pageStart={0}
                            loadMore={handleGetNext}
                            hasMore={!loadingNext && !!pagination && pagination.currentPage < pagination.totalPages}
                            initialLoad={false}
                        >
                            <DayPlanList />
                        </InfiniteScroll>
                    </Segment>
                )}

            </Grid.Column>
            <Grid.Column width='6'>
                Hello
                {/*<DayPlanFilters />*/}
            </Grid.Column>
            <Grid.Column width={10}>
                <Loader active={loadingNext} />
            </Grid.Column>
        </Grid>
    );
})

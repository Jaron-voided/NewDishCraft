import {Grid} from "semantic-ui-react";
import {useStore} from "../../../app/stores/store.ts";
import LoadingComponent from "../../../app/layout/LoadingComponent.tsx";
import {observer} from "mobx-react-lite";
import {useParams} from "react-router-dom";
import {useEffect} from "react";

import DayPlanDetailedHeader from "./DayPlanDetailedHeader.tsx";
import DayPlanDetailedInfo from "./DayPlanDetailedInfo.tsx";
import DayPlanDetailedSidebar from "./DayPlanDetailedSidebar.tsx";



export default observer(function DayPlanDetails() {
    const {dayPlanStore} = useStore();
    const {selectedDayPlan: dayPlan, loadDayPlan, loadingInitial} = dayPlanStore;
    const {id} = useParams();

    useEffect(() => {
        if (id) loadDayPlan(id);
    }, [id, loadDayPlan]);

    if (loadingInitial || !dayPlan) return <LoadingComponent />;

    return (
        <Grid>
            <Grid.Column width={10}>
                <DayPlanDetailedHeader dayPlan={dayPlan} />
                <DayPlanDetailedInfo dayPlan={dayPlan}/>
            </Grid.Column>
            <Grid.Column width={6}>
                <DayPlanDetailedSidebar />
            </Grid.Column>
        </Grid>
    )
})
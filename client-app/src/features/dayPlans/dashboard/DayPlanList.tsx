import {Header} from "semantic-ui-react";
import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";
import DayPlanListItem from "./DayPlanListItem.tsx";

export default observer(function DayPlanList() {
    const {dayPlanStore} = useStore();
    const {dayPlans} = dayPlanStore; // Use dayPlans directly for now

    // Group by month or week if needed later
    return (
        <>
            <Header sub color='teal'>
                Day Plans
            </Header>
            {dayPlans.map(dayPlan => (
                <DayPlanListItem key={dayPlan.id} dayPlan={dayPlan}/>
            ))}
        </>
    );
})

/*
import {Header} from "semantic-ui-react";
import {useStore} from "../../../app/stores/store.ts";
import {observer} from "mobx-react-lite";
import DayPlanListItem from "./DayPlanListItem.tsx";
import {Fragment} from "react";


export default observer(function DayPlanList() {
    const {dayPlanStore} = useStore();
    const {groupedDayPlans} = dayPlanStore;

    return (
        <>
            {groupedDayPlans.map(([name, dayPlans]) => (
                <Fragment>
                    <Header sub color='teal'>
                        {name}
                    </Header>
                    {dayPlans.map(dayPlan => (
                        <DayPlanListItem key={dayPlan.id} dayPlan={dayPlan}/>
                    ))}
                </Fragment>
            ))}
        </>

    );
})*/

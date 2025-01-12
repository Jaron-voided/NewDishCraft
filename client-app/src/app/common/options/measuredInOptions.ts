import { MeasuredIn } from "../../models/ingredient.ts";

export const measuredInOptions = [
    { key: MeasuredIn.Weight, text: "Weight", value: MeasuredIn.Weight },
    { key: MeasuredIn.Volume, text: "Volume", value: MeasuredIn.Volume },
    { key: MeasuredIn.Each, text: "Each", value: MeasuredIn.Each },
];

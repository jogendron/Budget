import { Frequency } from "./frequency";
import { Period } from "./period";

export interface SpendingCategory {
    id: string,
    name: string,
    period: Period,
    frequency: Frequency,
    amount: number,
    description: string
}
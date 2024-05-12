import { Frequency } from "./frequency";
import { Period } from "./period";

export interface NewSpendingCategory {
    name: string,
    frequency: Frequency,
    amount: number,
    description: string
}
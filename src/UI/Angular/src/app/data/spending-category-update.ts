import { Frequency } from "./frequency";

export interface SpendingCategoryUpdate {
    id: string,
    name: string,
    frequency: Frequency,
    isPeriodOpened: boolean,
    amount: number,
    description: string
}
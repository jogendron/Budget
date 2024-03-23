import { Routes } from '@angular/router';
import { SpendingCategoriesComponent } from './spending-categories/spending-categories.component';
import { SpendingsComponent } from './spendings/spendings.component';

export const routes: Routes = [
    {
        path: '',
        component: SpendingCategoriesComponent
    },
    {
        path: 'spending-categories',
        component: SpendingCategoriesComponent
    },
    {
        path: 'spendings',
        component: SpendingsComponent
    }
];

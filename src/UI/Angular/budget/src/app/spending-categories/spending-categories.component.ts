import { Component } from '@angular/core';
import { NgIf, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SpendingCategoriesService } from './spending-categories.service';

import { SpendingCategory } from '../data/spending-category';

@Component({
  selector: 'app-spending-categories',
  standalone: true,
  imports: [ NgIf, NgFor, FormsModule ],
  templateUrl: './spending-categories.component.html',
  styleUrl: './spending-categories.component.css'
})
export class SpendingCategoriesComponent {

  spendingCategories: SpendingCategory[] = [];

  searchInput: string = '';
  selectedCategory: string | undefined = undefined;

  constructor(private spendingCategoryService: SpendingCategoriesService) {
  }

  search() {
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    let searchWithGuid = guidRegex.test(this.searchInput);
    let component = this;

    this.spendingCategories = [];

    if (searchWithGuid) {

      this.spendingCategoryService.getSpendingCategory(this.searchInput).subscribe({
        next: value => {
          component.spendingCategories.push(value);
        },
        error: message => {
          console.error(message);
        }
      });

    } else if (this.searchInput.trim() != '') {

      this.spendingCategoryService.searchSpendingCategories(this.searchInput).subscribe({
        next: value => {
          component.spendingCategories = value;
        },
        error: message => {
          console.error(message);
        }
      });

    } else {

      this.spendingCategoryService.getSpendingCategories().subscribe({
        next: value => {
          component.spendingCategories = value;
        },
        error: message => {
          console.error(message);
        }
      });

    }
  }

  selectCategory(id: string) {
    this.selectedCategory = id;
  }

}

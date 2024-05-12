import { Component, OnInit } from '@angular/core';
import { NgIf, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SpendingCategoriesComponentState } from './spending-categories-component-state';
import { SpendingCategoriesService } from './spending-categories.service';
import { SpendingCategory } from '../data/spending-category';
import { ReadSpendingCategoryComponent } from './read-spending-category.component';
import { EditSpendingCategoryComponent } from './edit-spending-category.component';
import { CreateSpendingCategoryComponent } from './create-spending-category.component';
import { SpendingCategoryUpdate } from '../data/spending-category-update';
import { firstValueFrom } from 'rxjs';
import { NewSpendingCategory } from '../data/new-spending-category';

@Component({
  selector: 'app-spending-categories',
  standalone: true,
  imports: [ 
    ReadSpendingCategoryComponent, 
    EditSpendingCategoryComponent, 
    CreateSpendingCategoryComponent, 
    NgIf, 
    NgFor, 
    FormsModule 
  ],
  templateUrl: './spending-categories.component.html',
  styleUrl: './spending-categories.component.css'
})
export class SpendingCategoriesComponent implements OnInit {

  SpendingCategoriesComponentState = SpendingCategoriesComponentState;

  state: SpendingCategoriesComponentState;
  spendingCategories: SpendingCategory[];
  selectedCategory: SpendingCategory | undefined;

  constructor(private spendingCategoryService: SpendingCategoriesService) {
    this.state = SpendingCategoriesComponentState.reading;
    this.spendingCategories = [];
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  private loadCategories(idToSelect?: string) {
    let component = this;

    this.spendingCategoryService.getSpendingCategories().subscribe({
      next: value => {
        component.spendingCategories = value.sort(
          (a,b) => a.name.localeCompare(b.name)
        );

        if (component.spendingCategories) {
          if (idToSelect) {
            component.selectCategory(idToSelect);
          } else {
            component.selectedCategory = component.spendingCategories[0];
          }
        }
      },
      error: message => {
        console.error(message);
      }
    });
  }

  selectCategory(id: string) {
    this.state = SpendingCategoriesComponentState.reading;
    this.selectedCategory = this.spendingCategories.find(category => category.id == id);
  }

  createRequested() {
    this.state = SpendingCategoriesComponentState.creating;
  }

  createCancelled() {
    this.state = SpendingCategoriesComponentState.reading;
    this.selectedCategory = this.spendingCategories[0];
  }

  createCompleted(newCategory: NewSpendingCategory) {
    this.state = SpendingCategoriesComponentState.reading;

    firstValueFrom(
      this.spendingCategoryService.createSpendingCategory(newCategory)
    ).then(response => {
      this.loadCategories();
    });
  }

  editRequested() {
    this.state = SpendingCategoriesComponentState.updating;
  }

  editCancelled() {
    this.state = SpendingCategoriesComponentState.reading;
  }

  deleteRequested(category: SpendingCategory) {
    this.spendingCategoryService.deleteSpendingCategory(category).subscribe({
      next: () => {
        this.loadCategories();
      }
    });
  }

  editCompleted(category: SpendingCategory) {
    let update: SpendingCategoryUpdate = {
      id: category.id,
      name: category.name,
      frequency: category.frequency,
      isPeriodOpened: true,
      amount: category.amount,
      description: category.description
    }

    firstValueFrom(
      this.spendingCategoryService.updateSpendingCategory(update)
    ).then(response => {
      this.loadCategories(category.id);
      this.state = SpendingCategoriesComponentState.reading;
    });
  }

}

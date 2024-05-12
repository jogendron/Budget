import { Component, EventEmitter, Output } from '@angular/core';
import { NgIf } from '@angular/common';
import { NewSpendingCategory } from '../data/new-spending-category';
import { FormsModule } from '@angular/forms';
import { Frequency } from '../data/frequency';

@Component({
  selector: 'app-create-spending-category',
  standalone: true,
  imports: [ NgIf, FormsModule ],
  templateUrl: './create-spending-category.component.html',
  styleUrl: './create-spending-category.component.css'
})
export class CreateSpendingCategoryComponent {

  _newCategory: NewSpendingCategory | undefined;

  @Output() cancelled: EventEmitter<void>;
  @Output() saved: EventEmitter<NewSpendingCategory>;

  constructor() {
    this._newCategory = {
      name: '',
      frequency: Frequency.Monthly,
      amount: 0,
      description: ''
    };

    this.cancelled = new EventEmitter<void>();
    this.saved = new EventEmitter<NewSpendingCategory>();
  }

  cancel() {
    this.cancelled.emit();
  }

  save() {
    this.saved.emit(this._newCategory);
  }
  
}

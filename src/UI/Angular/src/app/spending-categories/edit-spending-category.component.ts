import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SpendingCategory } from '../data/spending-category';

@Component({
  selector: 'app-edit-spending-category',
  standalone: true,
  imports: [FormsModule, NgIf],
  templateUrl: './edit-spending-category.component.html',
  styleUrl: './edit-spending-category.component.css'
})
export class EditSpendingCategoryComponent {

  _category: SpendingCategory | undefined;

  @Output() saved: EventEmitter<SpendingCategory>;
  @Output() cancelled: EventEmitter<void>;

  constructor() {
    this.saved = new EventEmitter<SpendingCategory>();
    this.cancelled = new EventEmitter<void>();
  }

  @Input() set category(value: SpendingCategory | undefined) {
    if (value) {
      this._category = { ...value };
    }
  }

  save() {
    this.saved.emit(this._category);
  }

  cancel() {
    this.cancelled.emit();
  }

}

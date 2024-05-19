import { Component, EventEmitter, Output } from '@angular/core';
import { NgIf } from '@angular/common';
import { NewSpendingCategory } from '../data/new-spending-category';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-create-spending-category',
  standalone: true,
  imports: [ NgIf, ReactiveFormsModule ],
  templateUrl: './create-spending-category.component.html',
  styleUrl: './create-spending-category.component.css'
})
export class CreateSpendingCategoryComponent {

  categoryForm: FormGroup;

  @Output() cancelled: EventEmitter<void>;
  @Output() saved: EventEmitter<NewSpendingCategory>;

  constructor(formBuilder: FormBuilder) {
    this.categoryForm = formBuilder.group({
      name: ['', [
        Validators.required,
        Validators.maxLength(100)
      ]],
      description: ['', [
        Validators.required,
        Validators.maxLength(500)
      ]],
      amount: ['', [
        Validators.required,
        Validators.min(0)
      ]],
      frequency: ['', Validators.required]
    });

    this.cancelled = new EventEmitter<void>();
    this.saved = new EventEmitter<NewSpendingCategory>();
  }

  protected get name(): AbstractControl<any, any> | null {
    return this.categoryForm.get('name');
  }

  protected get description(): AbstractControl<any, any> | null {
    return this.categoryForm.get('description');
  }

  protected get amount(): AbstractControl<any, any> | null {
    return this.categoryForm.get('amount');
  }

  protected get frequency(): AbstractControl<any, any> | null {
    return this.categoryForm.get('frequency');
  }

  cancel() {
    this.cancelled.emit();
  }

  save() {
    if (this.categoryForm.valid) {
      let category = {
        name: this.name?.value,
        frequency: this.frequency?.value,
        amount: this.amount?.value,
        description: this.description?.value
      };
  
      this.saved.emit(category);
    }
    else {
      this.categoryForm.markAllAsTouched();
    } 
  }
  
}

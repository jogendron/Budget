import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgIf } from '@angular/common';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SpendingCategory } from '../data/spending-category';
import { SpendingCategoryUpdate } from '../data/spending-category-update';

@Component({
  selector: 'app-edit-spending-category',
  standalone: true,
  imports: [ NgIf, ReactiveFormsModule ],
  templateUrl: './edit-spending-category.component.html',
  styleUrl: './edit-spending-category.component.css'
})
export class EditSpendingCategoryComponent {

  updateForm: FormGroup;
  update: SpendingCategoryUpdate | undefined;

  @Output() saved: EventEmitter<SpendingCategoryUpdate>;
  @Output() cancelled: EventEmitter<void>;

  constructor(formBuilder: FormBuilder) {
    this.updateForm = formBuilder.group({
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

    this.saved = new EventEmitter<SpendingCategoryUpdate>();
    this.cancelled = new EventEmitter<void>();
  }

  @Input() set category(value: SpendingCategory | undefined) {
    if (value) {
      this.update = {
        id: value.id,
        name: value.name,
        frequency: value.frequency,
        isPeriodOpened: true,
        amount: value.amount,
        description: value.description
      };

      this.name?.setValue(value.name);
      this.description?.setValue(value.description);
      this.amount?.setValue(value.amount);
      this.frequency?.setValue(value.frequency);
    }
  }

  protected get name(): AbstractControl<any, any> | null {
    return this.updateForm.get('name');
  }

  protected get description(): AbstractControl<any, any> | null {
    return this.updateForm.get('description');
  }

  protected get amount(): AbstractControl<any, any> | null {
    return this.updateForm.get('amount');
  }

  protected get frequency(): AbstractControl<any, any> | null {
    return this.updateForm.get('frequency');
  }

  cancel() {
    this.cancelled.emit();
  }

  save() {
    if (this.updateForm.valid) {
      if (this.update) {
        this.update.name = this.name?.value;
        this.update.description = this.description?.value;
        this.update.amount = this.amount?.value;
        this.update.frequency = this.frequency?.value;
      }
  
      this.saved.emit(this.update);
    } else {
      this.updateForm.markAllAsTouched();
    }
  }

}
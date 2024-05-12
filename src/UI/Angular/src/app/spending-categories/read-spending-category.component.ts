import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgIf, CurrencyPipe, DatePipe } from '@angular/common';
import { SpendingCategory } from '../data/spending-category';
import { TranslateService, TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-read-spending-category',
  standalone: true,
  imports: [ NgIf, CurrencyPipe, DatePipe, TranslateModule ],
  templateUrl: './read-spending-category.component.html',
  styleUrl: './read-spending-category.component.css'
})
export class ReadSpendingCategoryComponent {

  translateService: TranslateService;

  @Input() category: SpendingCategory | undefined;

  @Output() editRequested: EventEmitter<void>;
  @Output() deleteRequested: EventEmitter<SpendingCategory>;

  constructor(translateService: TranslateService) {
    this.translateService = translateService;
    this.translateService.setDefaultLang('en');
    this.translateService.use('fr');

    this.editRequested = new EventEmitter<void>();
    this.deleteRequested = new EventEmitter<SpendingCategory>();
  }

  edit() {
    this.editRequested.emit();
  }

  delete() {
    this.deleteRequested.emit(this.category);
  }

}

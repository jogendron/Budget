import { Component, EventEmitter, Input, Output, Inject, LOCALE_ID } from '@angular/core';
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

  @Input() category: SpendingCategory | undefined;

  @Output() editRequested: EventEmitter<void>;
  @Output() deleteRequested: EventEmitter<SpendingCategory>;

  constructor(
    public translateService: TranslateService,
    @Inject(LOCALE_ID) private locale: string
  ) {
    this.translateService = translateService;
    this.translateService.setDefaultLang('en-US');
    this.translateService.use(locale);

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

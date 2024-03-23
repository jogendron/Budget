import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpendingCategoriesComponent } from './spending-categories.component';

describe('SpendingCategoriesComponent', () => {
  let component: SpendingCategoriesComponent;
  let fixture: ComponentFixture<SpendingCategoriesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SpendingCategoriesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SpendingCategoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

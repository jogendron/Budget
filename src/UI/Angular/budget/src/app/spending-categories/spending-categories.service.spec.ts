import { TestBed } from '@angular/core/testing';

import { SpendingCategoriesService } from './spending-categories.service';

describe('SpendingCategoriesService', () => {
  let service: SpendingCategoriesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SpendingCategoriesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

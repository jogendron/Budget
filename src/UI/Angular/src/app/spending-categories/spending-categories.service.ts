import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { SpendingCategory } from '../data/spending-category';
import { SpendingCategoryUpdate } from '../data/spending-category-update';
import { NewSpendingCategory } from '../data/new-spending-category';

import config from '../../assets/configuration.json'

@Injectable({
  providedIn: 'root'
})
export class SpendingCategoriesService {

  private apiUrl: string = config.apiConfiguration.spendingCategoriesUrl;
  private header: {} = {};

  constructor(private http: HttpClient, private securityService: OidcSecurityService) {
    this.securityService.getAccessToken().subscribe({
      next: value => {
        this.header = {
          "Authorization": `Bearer ${value}`
        }
      },
      error: message => {
        console.error(message);
      }
    });
  }

  getSpendingCategory(id: string): Observable<SpendingCategory> {
    return this.http.get<SpendingCategory>(`${this.apiUrl}/${id}`, {
      headers: this.header
    });
  }

  getSpendingCategories(): Observable<SpendingCategory[]> {
    return this.http.get<SpendingCategory[]>(this.apiUrl, {
      headers: this.header
    });
  }

  searchSpendingCategories(name: string): Observable<SpendingCategory[]> {
    return this.http.get<SpendingCategory[]>(`${this.apiUrl}?name=${name}`, {
      headers: this.header
    });
  }

  createSpendingCategory(newCategory: NewSpendingCategory): Observable<void> {
    return this.http.post<void>(
      `${this.apiUrl}`,
      newCategory,
      {
        headers: this.header
      }
    );
  }

  updateSpendingCategory(update: SpendingCategoryUpdate): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}`,
      update,
      {
        headers: this.header
      }
    );
  }

  deleteSpendingCategory(category: SpendingCategory): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${category.id}`,
      {
        headers: this.header
      }
    );
  }

}

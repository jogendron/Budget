import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { SpendingCategory } from '../data/spending-category';

@Injectable({
  providedIn: 'root'
})
export class SpendingCategoriesService {

  private apiUrl: string = 'https://denethor:7154/api/v1/SpendingCategories';
  private token: string | undefined = undefined;

  constructor(private http: HttpClient, private securityService: OidcSecurityService) { 
    this.securityService.getAccessToken().subscribe({
      next: value => {
        this.token = value;
      },
      error: message => {
        console.error(message);
      }
    });
  }

  getSpendingCategory(id: string): Observable<SpendingCategory> {
    return this.http.get<SpendingCategory>(`{this.apiUrl}/{id}`);
  }

  getSpendingCategories(): Observable<SpendingCategory[]> {
    return this.http.get<SpendingCategory[]>(this.apiUrl);
  }  

  searchSpendingCategories(name: string) : Observable<SpendingCategory[]> {
    return this.http.get<SpendingCategory[]>(`{this.apiUrl}/{name}`);
  }
  
}

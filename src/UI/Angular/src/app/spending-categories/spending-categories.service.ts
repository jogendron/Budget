import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigurationService } from '../configuration/configuration.service';
import { ApiConfiguration } from '../configuration/api-configuration';
import { Observable, firstValueFrom, map } from 'rxjs';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { SpendingCategory } from '../data/spending-category';
import { SpendingCategoryUpdate } from '../data/spending-category-update';
import { NewSpendingCategory } from '../data/new-spending-category';
import { Configuration } from '../configuration/configuration';

@Injectable({
  providedIn: 'root'
})
export class SpendingCategoriesService {

  private apiUrl: string = '';
  private header: {} = {};

  constructor(
    private http: HttpClient, 
    private configService: ConfigurationService,
    private securityService: OidcSecurityService
  ) {
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

  async getSpendingCategory(id: string): Promise<SpendingCategory> {
    let apiConfig = await firstValueFrom(this.configService.getApiConfiguration());

    return firstValueFrom(
      this.http.get<SpendingCategory>(
        `${apiConfig.spendingCategoriesUrl}/${id}`, 
        { headers: this.header }
      )
    );
  }

  async getSpendingCategories(): Promise<SpendingCategory[]> {
    let apiConfig = await firstValueFrom(this.configService.getApiConfiguration());

    return firstValueFrom(
      this.http.get<SpendingCategory[]>(
        apiConfig.spendingCategoriesUrl, 
        { headers: this.header }
      )
    );
  }

  async searchSpendingCategories(name: string): Promise<SpendingCategory[]> {
    let apiConfig = await firstValueFrom(this.configService.getApiConfiguration());

    return firstValueFrom(
      this.http.get<SpendingCategory[]>(
        `${apiConfig.spendingCategoriesUrl}?name=${name}`, 
        { headers: this.header }
      )
    );
  }

  async createSpendingCategory(newCategory: NewSpendingCategory): Promise<void> {
    let apiConfig = await firstValueFrom(this.configService.getApiConfiguration());

    return firstValueFrom(
      this.http.post<void>(
        `${apiConfig.spendingCategoriesUrl}`,
        newCategory,
        { headers: this.header }
    ));
  }

  async updateSpendingCategory(update: SpendingCategoryUpdate): Promise<void> {
    let apiConfig = await firstValueFrom(this.configService.getApiConfiguration());

    return firstValueFrom(
      this.http.patch<void>(
        `${apiConfig.spendingCategoriesUrl}`,
        update,
        { headers: this.header }
      )
    );
  }

  async deleteSpendingCategory(category: SpendingCategory): Promise<void> {
    let apiConfig = await firstValueFrom(this.configService.getApiConfiguration());

    return firstValueFrom(
      this.http.delete<void>(
        `${apiConfig.spendingCategoriesUrl}/${category.id}`,
        { headers: this.header }
      )
    );
  }

}

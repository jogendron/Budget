import { Injectable, Inject, isDevMode, LOCALE_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Configuration } from './configuration';
import { OpenIdConfiguration } from 'angular-auth-oidc-client';
import { ApiConfiguration } from './api-configuration';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {

  private configUrl: string = '/assets/configuration.json';

  constructor(
    private http: HttpClient, 
    @Inject(LOCALE_ID) locale:string
  ) {
    if (! isDevMode()) {
      this.configUrl = `/${locale}${this.configUrl}`;
    }
  }

  getConfiguration() : Observable<Configuration> {
    return this.http.get<Configuration>(this.configUrl);
  }

  getOpenIdConfiguration() : Observable<OpenIdConfiguration> {
    return this.http.get<Configuration>(this.configUrl).pipe(
      map((value: Configuration) => {
        return value.openIdConfiguration;
      })
    );
  }
  
  getApiConfiguration() : Observable<ApiConfiguration> {
    return this.http.get<Configuration>(this.configUrl).pipe(
      map((value: Configuration) => {
        return value.apiConfiguration;
      })
    );
  }

}

import { PassedInitialConfig } from 'angular-auth-oidc-client';

export const authConfig: PassedInitialConfig = {
  config: {
    authority: 'https://denethor:8443/realms/Denethor',
    redirectUrl: 'http://localhost:4200/spending-categories',
    postLogoutRedirectUri: 'http://localhost:4200',
    clientId: 'budget-angular-ui',
    scope: 'openid profile offline_access Spendings.Read Spendings.Write',
    responseType: 'code',
    silentRenew: true,
    useRefreshToken: true,
    renewTimeBeforeTokenExpiresInSeconds: 30
  }
}

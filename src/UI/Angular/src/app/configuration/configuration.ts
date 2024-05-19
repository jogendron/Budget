import { OpenIdConfiguration } from 'angular-auth-oidc-client';
import { ApiConfiguration } from './api-configuration';

export interface Configuration {
    openIdConfiguration: OpenIdConfiguration,
    apiConfiguration: ApiConfiguration
}
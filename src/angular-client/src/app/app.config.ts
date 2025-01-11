import {
  ApplicationConfig,
  provideZoneChangeDetection
} from '@angular/core';
import {provideRouter, withComponentInputBinding, withViewTransitions} from '@angular/router';

import { routes } from './app.routes';
import {
  provideHttpClient, withInterceptors,
  withInterceptorsFromDi
} from '@angular/common/http';
import {
  createInterceptorCondition,
  INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG, IncludeBearerTokenCondition, includeBearerTokenInterceptor,
  provideKeycloak,
} from 'keycloak-angular';
import {provideAnimations} from '@angular/platform-browser/animations';
import {gsap, TextPlugin} from 'gsap/all'
import {environment} from '../environments/environment';
const provideKeycloakAngular  = () => provideKeycloak({
  config: {
    url: 'https://easy-auth.ru/keycloak',
    realm: environment.keycloakRealm,
    clientId: 'frontend'
  },
  initOptions: {
    onLoad: 'check-sso',
    checkLoginIframe: false,
  },
  providers: [
    {
      provide: INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG,
      useValue: [urlCondition]
    }
  ],
})
const urlCondition = createInterceptorCondition<IncludeBearerTokenCondition>({
  urlPattern: /^\/api/,
  bearerPrefix: 'Bearer'
});
gsap.registerPlugin(TextPlugin)
export const appConfig: ApplicationConfig = {
  providers: [
    provideKeycloakAngular(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideHttpClient(withInterceptorsFromDi(),withInterceptors([includeBearerTokenInterceptor])),
    provideRouter(routes, withViewTransitions(), withComponentInputBinding()),
    provideAnimations()]
};


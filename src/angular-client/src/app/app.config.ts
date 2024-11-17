import {
  APP_INITIALIZER,
  ApplicationConfig,
  Provider,
  provideZoneChangeDetection
} from '@angular/core';
import {provideRouter, withComponentInputBinding, withViewTransitions} from '@angular/router';

import { routes } from './app.routes';
import {
  HTTP_INTERCEPTORS,
  provideHttpClient,
  withInterceptorsFromDi
} from '@angular/common/http';
import {KeycloakBearerInterceptor, KeycloakService} from 'keycloak-angular';
import {provideAnimations} from '@angular/platform-browser/animations';

function initializeKeycloak(keycloak: KeycloakService) {
  return () =>
    keycloak.init({
      config: {
        url: 'https://easy-auth.ru/keycloak',
        realm: 'easy-quiz-test',
        clientId: 'frontend'
      },
      initOptions: {
        onLoad: 'check-sso',
        checkLoginIframe: false
      },
      bearerExcludedUrls: [],
      loadUserProfileAtStartUp: false,
      enableBearerInterceptor: true,
      bearerPrefix: 'Bearer',
    });
}

const KeycloakBearerInterceptorProvider: Provider = {
  provide: HTTP_INTERCEPTORS,
  useClass: KeycloakBearerInterceptor,
  multi: true
};

const KeycloakInitializerProvider: Provider = {
  provide: APP_INITIALIZER,
  useFactory: initializeKeycloak,
  multi: true,
  deps: [KeycloakService]
}
export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideHttpClient(withInterceptorsFromDi()),
    KeycloakInitializerProvider,
    KeycloakBearerInterceptorProvider,
    KeycloakService,
    provideRouter(routes,withViewTransitions(),withComponentInputBinding()),
    provideAnimations()]
};


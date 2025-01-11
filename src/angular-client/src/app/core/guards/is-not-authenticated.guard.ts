import {CanActivateFn, Router} from '@angular/router';
import {inject} from '@angular/core';
import Keycloak from 'keycloak-js';

export const isNotAuthenticatedGuard: CanActivateFn = async (route, state) => {
  const keycloak = inject(Keycloak);
  const router = inject(Router);
  if(!keycloak.authenticated)
    return true;
  await router.navigate(['/'])
  return false;
};

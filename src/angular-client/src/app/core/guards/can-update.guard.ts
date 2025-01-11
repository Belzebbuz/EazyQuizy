import {CanActivateFn, Router} from '@angular/router';
import {inject} from '@angular/core';
import {QuizService} from '../services/quiz.service';
import {GrainAuthService} from '../services/grain-auth.service';

export const canUpdateGuard: CanActivateFn = async (route, state) => {
  const service = inject(GrainAuthService);
  const router = inject(Router);
  const isAuthorized = await service.isAuthorized(route.params['id'], 'update');
  if(isAuthorized.authorized)
    return true;
  await router.navigate(['/'])
  return false;
};

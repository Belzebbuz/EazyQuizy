import { Routes } from '@angular/router';
import {WelcomePageComponent} from './features/welcome-page/welcome-page.component';
import {MainComponent} from './features/main/main.component';
import {isAuthenticatedGuard} from './core/guards/is-authenticated.guard';
import {isNotAuthenticatedGuard} from './core/guards/is-not-authenticated.guard';
import {CreateQuizComponent} from './features/create-quiz/create-quiz.component';
import {ConfigureQuizComponent} from './features/configure-quiz/configure-quiz.component';
import {canUpdateGuard} from './core/guards/can-update.guard';
import {
  ConfigureSingleAnswerQuestionComponent
} from './features/configure-quiz/configure-single-answer-question/configure-single-answer-question.component';
import {
  ConfigureMultipleAnswerQuestionComponent
} from './features/configure-quiz/configure-multiple-answer-question/configure-multiple-answer-question.component';
import {
  ConfigureOrderQuestionComponent
} from './features/configure-quiz/configure-order-question/configure-order-question.component';
import {
  ConfigureRangeQuestionComponent
} from './features/configure-quiz/configure-range-question/configure-range-question.component';
import {LobbyComponent} from './features/lobby/lobby.component';
import {GameComponent} from './features/game/game.component';

export const routes: Routes = [
  {
    path: '',
    component: MainComponent,
    data: { animation: 'Main'},
    canActivate: [isAuthenticatedGuard],
  },
  {
    path: 'welcome-page',
    component: WelcomePageComponent,
    canActivate: [isNotAuthenticatedGuard]
  },
  {
    path: 'create',
    component: CreateQuizComponent,
    canActivate: [isAuthenticatedGuard]
  },
  {
    path: 'configure/:id',
    component: ConfigureQuizComponent,
    canActivate: [canUpdateGuard, isAuthenticatedGuard]
  },
  {
    path: 'configure/:id/single-answer',
    component: ConfigureSingleAnswerQuestionComponent,
    canActivate: [canUpdateGuard, isAuthenticatedGuard]
  },
  {
    path: 'configure/:id/single-answer/:questionId',
    component: ConfigureSingleAnswerQuestionComponent,
    canActivate: [canUpdateGuard, isAuthenticatedGuard]
  },
  {
    path: 'configure/:id/multiple-answer',
    component: ConfigureMultipleAnswerQuestionComponent,
    canActivate: [canUpdateGuard, isAuthenticatedGuard]
  },
  {
    path: 'configure/:id/multiple-answer/:questionId',
    component: ConfigureMultipleAnswerQuestionComponent,
    canActivate: [canUpdateGuard, isAuthenticatedGuard]
  },
  {
    path: 'configure/:id/order',
    component: ConfigureOrderQuestionComponent,
    canActivate: [canUpdateGuard, isAuthenticatedGuard]
  },
  {
    path: 'configure/:id/order/:questionId',
    component: ConfigureOrderQuestionComponent,
    canActivate: [canUpdateGuard, isAuthenticatedGuard]
  },
  {
    path: 'configure/:id/range',
    component: ConfigureRangeQuestionComponent,
    canActivate: [canUpdateGuard, isAuthenticatedGuard]
  },
  {
    path: 'configure/:id/range/:questionId',
    component: ConfigureRangeQuestionComponent,
    canActivate: [canUpdateGuard, isAuthenticatedGuard]
  },
  {
    path: 'lobby/:id',
    component: LobbyComponent,
  },
  {
    path: 'game/:id',
    component: GameComponent,
  }
];

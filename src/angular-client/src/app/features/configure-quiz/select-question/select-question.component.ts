import {Component, Input} from '@angular/core';
import {RouterLink} from '@angular/router';
import {HoveredBtnComponent} from '../../../core/motions/hovered-btn/hovered-btn.component';
import {ContentStaggerDirective} from '../../../core/motions/directives/content-stagger.directive';
import {SlideTransitionDirective} from '../../../core/motions/directives/slide-transition.directive';

@Component({
  selector: 'app-select-question',
  imports: [
    RouterLink,
    HoveredBtnComponent,
    ContentStaggerDirective,
    SlideTransitionDirective
  ],
  templateUrl: './select-question.component.html',
})
export class SelectQuestionComponent  {
  @Input() quizId?: string;
}

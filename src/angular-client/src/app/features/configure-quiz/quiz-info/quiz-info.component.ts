import {Component, inject, Input} from '@angular/core';
import {NgForOf, NgIf} from '@angular/common';
import {QuestionShortInfo, QuestionType, QuizInfo} from '../../../../generated/types/types';
import {TypewriterComponent} from '../../../core/motions/typewriter/typewriter.component';
import {ContentStaggerDirective} from '../../../core/motions/directives/content-stagger.directive';
import {SlideTransitionDirective} from '../../../core/motions/directives/slide-transition.directive';
import {CdkDrag, CdkDragDrop, CdkDragHandle, CdkDropList, moveItemInArray} from '@angular/cdk/drag-drop';
import {ArrowsPointingOutComponent} from '../../../core/icons/arrows-pointing-out/arrows-pointing-out.component';
import {PencilSquareComponent} from '../../../core/icons/pencil-square/pencil-square.component';
import {TrashComponent} from '../../../core/icons/trash/trash.component';
import {QuizService} from '../../../core/services/quiz.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-quiz-info',
  imports: [
    NgIf,
    NgForOf,
    TypewriterComponent,
    ContentStaggerDirective,
    SlideTransitionDirective,
    CdkDropList,
    CdkDrag,
    CdkDragHandle,
    ArrowsPointingOutComponent,
    PencilSquareComponent,
    TrashComponent,
  ],
  templateUrl: './quiz-info.component.html'
})
export class QuizInfoComponent{
  @Input() quiz?: QuizInfo;
  @Input() questions: QuestionShortInfo[] = [];
  @Input() onDelete?: () => Promise<void>;
  service = inject(QuizService);
  router = inject(Router);
  canDrag = true;
  async drop(event: CdkDragDrop<QuestionShortInfo[]>) {
    if(!this.quiz || !this.canDrag) return;
    this.canDrag = false;
    const oldQuestionOrder = this.questions![event.currentIndex].order;
    const questionId = this.questions![event.previousIndex].id;
    moveItemInArray(this.questions!, event.previousIndex, event.currentIndex);
    const result = await this.service.setQuestionNewOrder(this.quiz.id, questionId,oldQuestionOrder);
    if(result.succeeded){
      const info = await this.service.getInfo(this.quiz.id);
      this.questions = info.questions
    }else{
      moveItemInArray(this.questions!, event.currentIndex, event.previousIndex);
    }
    this.canDrag = true;
  }

  async routeToEdit(question: QuestionShortInfo){
    const basePath ='/configure/' + this.quiz?.id;
    if (question.questionType == QuestionType.MultipleAnswers)
      await this.router.navigate([ basePath + '/multiple-answer/' + question.id])
    if (question.questionType == QuestionType.SingleAnswer)
      await this.router.navigate([ basePath + '/single-answer/' + question.id])
    if (question.questionType == QuestionType.RangeAnswer)
      await this.router.navigate([ basePath + '/range/' + question.id])
    if (question.questionType == QuestionType.OrderAnswers)
      await this.router.navigate([ basePath + '/order/' + question.id])
  }

  async delete(questionId: string){
    if(!this.quiz) return;
    const result = await this.service.deleteQuestion(this.quiz.id, questionId);
    if(result.succeeded && this.onDelete)
      await this.onDelete();
  }

  getQuestionType(info: QuestionShortInfo){
    if (info.questionType == QuestionType.SingleAnswer)
      return 'Выбор одного варианта ответа'
    if (info.questionType == QuestionType.MultipleAnswers)
      return 'Выбор нескольких вариантов ответа'
    if (info.questionType == QuestionType.OrderAnswers)
      return 'Указать правильный порядок'
    if (info.questionType == QuestionType.RangeAnswer)
      return 'Выбор из диапазона'
    return '';
  }
}

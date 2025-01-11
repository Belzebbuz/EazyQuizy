import {Component, inject, Input, OnInit} from '@angular/core';
import {GamePlayerInfo, GameOrderQuestionInfo, SetAnswerRequest} from '../../../../generated/quiz/game';
import {GameService} from '../../../core/services/game.service';
import {GameImageComponent} from '../game-image/game-image.component';
import {NgForOf, NgIf} from '@angular/common';
import {ArrowComponent} from '../../../core/icons/arrow/arrow.component';
import {OrderedValue, QuestionShortInfo} from '../../../../generated/types/types';
import {ArrowsPointingOutComponent} from '../../../core/icons/arrows-pointing-out/arrows-pointing-out.component';
import {CdkDrag, CdkDragDrop, CdkDragHandle, CdkDropList, moveItemInArray} from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-game-order-answer',
  imports: [
    GameImageComponent,
    NgForOf,
    NgIf,
    ArrowComponent,
    ArrowsPointingOutComponent,
    CdkDragHandle,
    CdkDropList,
    CdkDrag
  ],
  templateUrl: './game-order-answer.component.html',
})
export class GameOrderAnswerComponent{
  @Input() question?: GameOrderQuestionInfo;
  @Input() gameId!: string;
  @Input() player!: GamePlayerInfo;
  service = inject(GameService)
  isLoading = false;

  drop(event: CdkDragDrop<string[]>) {
    if(!this.question) return;
    moveItemInArray(this.question.answers, event.previousIndex, event.currentIndex);
  }
  up(index: number){
    if(!this.question) return;
    if (index > 0) {
      [this.question.answers[index - 1], this.question.answers[index]] = [this.question.answers[index], this.question.answers[index - 1]];
    }
  }
  down(index: number): void {
    if(!this.question) return;
    if (index < this.question.answers.length - 1) {
      [this.question.answers[index], this.question.answers[index + 1]] = [this.question.answers[index + 1], this.question.answers[index]];
    }
  }

  async setAnswer(){
    if(!this.question) return;
    this.isLoading = true;
    const request = SetAnswerRequest.create();
    request.gameId = this.gameId;
    request.orderedAnswer = this.question?.answers.map(((x, index) => {
      const answer = OrderedValue.create();
      answer.value = x;
      answer.order = index;
      return answer;
    }));
    await this.service.setAnswer(request);
    this.isLoading = false;
  }
}

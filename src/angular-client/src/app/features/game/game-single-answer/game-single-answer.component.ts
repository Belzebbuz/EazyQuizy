import {Component, inject, Input} from '@angular/core';
import {GamePlayerInfo, GameSingleAnswerQuestionInfo, SetAnswerRequest} from '../../../../generated/quiz/game';
import {GameService} from '../../../core/services/game.service';
import {GameImageComponent} from '../game-image/game-image.component';
import {NgForOf, NgIf} from '@angular/common';

@Component({
  selector: 'app-game-single-answer',
  imports: [
    GameImageComponent,
    NgForOf,
    NgIf
  ],
  templateUrl: './game-single-answer.component.html',
})
export class GameSingleAnswerComponent {
  @Input() question?: GameSingleAnswerQuestionInfo
  @Input() gameId!: string;
  @Input() player!: GamePlayerInfo;
  currentAnswer?: string;
  service = inject(GameService)
  isLoading = false;
  async setAnswer(){
    if(!this.currentAnswer) return;
    this.isLoading = true;
    const request = SetAnswerRequest.create();
    request.gameId = this.gameId;
    request.singleAnswer = this.currentAnswer;
    await this.service.setAnswer(request);
    this.isLoading = false;
  }
}

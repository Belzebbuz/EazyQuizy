import {Component, inject, Input} from '@angular/core';
import {GamePlayerInfo, GameMultipleAnswerQuestionInfo, SetAnswerRequest} from '../../../../generated/quiz/game';
import {NgForOf, NgIf} from '@angular/common';
import {GameImageComponent} from '../game-image/game-image.component';
import {GameService} from '../../../core/services/game.service';

@Component({
  selector: 'app-game-multiple-answer',
  imports: [
    NgIf,
    GameImageComponent,
    NgForOf
  ],
  templateUrl: './game-multiple-answer.component.html',
})
export class GameMultipleAnswerComponent {
  @Input() question?: GameMultipleAnswerQuestionInfo
  @Input() gameId!: string;
  @Input() player!: GamePlayerInfo;
  answers: string[] = [];
  service = inject(GameService)
  isLoading = false;
  addAnswer(answer: string){
    if(!this.answerAdded(answer))
      this.answers.push(answer)
    else
      this.answers = this.answers.filter(x => x != answer);
  }

  answerAdded(answer: string){
    return this.answers.findIndex(x => x == answer) > -1;
  }

  async setAnswer(){
    this.isLoading = true;
    const request = SetAnswerRequest.create();
    request.gameId = this.gameId;
    request.multipleAnswer = this.answers;
    await this.service.setAnswer(request);
    this.isLoading = false;
  }
}

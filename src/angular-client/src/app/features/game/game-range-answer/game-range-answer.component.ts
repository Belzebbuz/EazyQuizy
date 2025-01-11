import {Component, inject, Input, OnInit} from '@angular/core';
import {GamePlayerInfo, GameRangeQuestionInfo, SetAnswerRequest} from '../../../../generated/quiz/game';
import {GameService} from '../../../core/services/game.service';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {GameImageComponent} from '../game-image/game-image.component';
import {NgIf} from '@angular/common';
import {InputNumberComponent} from '../../../core/inputs/input-number/input-number.component';

@Component({
  selector: 'app-game-range-answer',
  imports: [
    GameImageComponent,
    NgIf,
    InputNumberComponent
  ],
  templateUrl: './game-range-answer.component.html',
})
export class GameRangeAnswerComponent implements OnInit {
  @Input() question?: GameRangeQuestionInfo;
  @Input() gameId!: string;
  @Input() player!: GamePlayerInfo;
  service = inject(GameService)
  isLoading = false;
  form?:  FormGroup<{answer: FormControl<number | null>}>;
  ngOnInit(): void {
    this.form = this.responseForm();
    }
  responseForm (){
    if(!this.question) return;
    return new FormGroup({
      answer: new FormControl(this.question.minValue, [Validators.required, Validators.min(this.question.minValue), Validators.max(this.question.maxValue)])
    })
  }

  async setAnswer(){
    if(!this.question || !this.form || this.form.invalid) return;
    this.isLoading = true;
    const request = SetAnswerRequest.create();
    request.gameId = this.gameId;
    request.rangeAnswer = this.form.controls.answer.value!;
    await this.service.setAnswer(request);
    this.isLoading = false;
  }
}


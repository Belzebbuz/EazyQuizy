import {ChangeDetectorRef, Component, inject, OnDestroy, OnInit} from '@angular/core';
import {GameService} from '../../core/services/game.service';
import {NatsService} from '../../core/services/nats.service';
import {ActivatedRoute, Router} from '@angular/router';
import {NgForOf, NgIf, NgSwitch, NgSwitchCase} from '@angular/common';
import {GameInfoStatus, GetGameInfoResponse} from '../../../generated/quiz/game';
import {GrainAuthService} from '../../core/services/grain-auth.service';
import {GameSingleAnswerComponent} from './game-single-answer/game-single-answer.component';
import {GameMultipleAnswerComponent} from './game-multiple-answer/game-multiple-answer.component';
import {GameRangeAnswerComponent} from './game-range-answer/game-range-answer.component';
import {GameOrderAnswerComponent} from './game-order-answer/game-order-answer.component';
import {GrainStateChangedEvent, QuestionType, TimerUpdateEvent} from '../../../generated/types/types';
import {gsap} from 'gsap'
import {MetadataService} from '../../core/services/metadata.service';
@Component({
  selector: 'app-game',
  imports: [
    NgForOf,
    NgIf,
    NgSwitch,
    GameSingleAnswerComponent,
    GameMultipleAnswerComponent,
    GameRangeAnswerComponent,
    GameOrderAnswerComponent,
    NgSwitchCase,
  ],
  templateUrl: './game.component.html',
})
export class GameComponent implements OnInit{
  gameId!: string;
  gameInfo?: GetGameInfoResponse;
  canStartGame = false;

  service = inject(GameService);
  nats = inject(NatsService);
  router = inject(Router);
  playerMetadata = inject(MetadataService);
  authService = inject(GrainAuthService)
  pointsLeft = 1000;
  playerId = this.playerMetadata.getPlayerId();
  constructor(private route: ActivatedRoute) {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.gameId = id;
  }

  get currentPlayer(){
    const player = this.gameInfo?.players.find(x => x.playerId == this.playerId);
    if(!player) throw new Error('Method not implemented.')
    return player;
  }
  get gameCreated() {
    return this.gameInfo?.status == GameInfoStatus.Created;
  }
  get gameStarted(){
    return this.gameInfo?.status == GameInfoStatus.Started
  }
  get gameCompleted(){
    return this.gameInfo?.status == GameInfoStatus.Completed
  }
  async ngOnInit() {
    try {
      this.gameInfo = await this.service.getInfo(this.gameId);
      const policy = await this.authService.isAuthorized(this.gameId, 'start-game');
      this.canStartGame = policy.authorized;
      await Promise.all([
        this.subscribeForUpdate(),
        this.subscribeForTimerUpdate()
      ])
    } catch {
      await this.router.navigate(['/'])
    }
  }

  private async subscribeForUpdate(){
    if(!this.gameInfo) return;
    for await (let _ of this.nats.subscribe<GrainStateChangedEvent>(
      this.gameInfo.updateChannel
    )){
      this.gameInfo = await this.service.getInfo(this.gameId);
    }
  }
  private async subscribeForTimerUpdate(){
    if(!this.gameInfo) return;
    for await (let timer of this.nats.subscribe<TimerUpdateEvent>(
      this.gameInfo.updateTimerChannel
    )){
      this.drawTimer(timer)
    }
  }

  private drawTimer(event: TimerUpdateEvent){
    const fullTimer = document.querySelector<HTMLElement>('.full-timer');
    const timer = document.querySelector<HTMLElement>('.timer');
    const timerCrl = document.querySelector<HTMLElement>('.timer-crl');
    if(!fullTimer || !timer || !timerCrl)
      return;
    const fullRect = fullTimer.getBoundingClientRect();
    const percent = event.value / event.startValue;
    this.pointsLeft = Math.round(percent * 1000);
    gsap.to(timer,{
      duration: 1,
      width: fullRect.width * percent
    })
    gsap.to(timerCrl, {
      duration: 1,
      x: -fullRect.width * (1 - percent)
    });
  }

  async play(){
    const result = await this.service.play(this.gameId);
  }

  protected readonly GameInfoStatus = GameInfoStatus;
  protected readonly QuestionType = QuestionType;
}

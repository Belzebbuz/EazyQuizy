import {Component, inject, OnInit} from '@angular/core';
import {QuizService} from '../../core/services/quiz.service';
import {PageInfo, QuizInfo} from '../../../generated/types/types';
import {TypewriterComponent} from '../../core/motions/typewriter/typewriter.component';
import {MotionEvent, MotionService} from '../../core/services/motion.service';
import {Router, RouterLink} from '@angular/router';
import {NgForOf, NgIf} from '@angular/common';
import {ContentStaggerDirective} from '../../core/motions/directives/content-stagger.directive';
import {TrashComponent} from '../../core/icons/trash/trash.component';
import {PencilSquareComponent} from '../../core/icons/pencil-square/pencil-square.component';
import {LobbyService} from '../../core/services/lobby.service';

@Component({
  selector: 'app-main',
  imports: [
    TypewriterComponent,
    RouterLink,
    NgIf,
    NgForOf,
    ContentStaggerDirective,
    TrashComponent,
    PencilSquareComponent,
  ],
  templateUrl: './main.component.html'
})
export class MainComponent implements OnInit {
  service = inject(QuizService);
  lobbyService = inject(LobbyService);
  motionService = inject(MotionService);
  router = inject(Router)
  quizzes: QuizInfo[] = [];
  pageInfo?: PageInfo;
  quizWords = ['easy quiz!', 'quiz!', 'квиз!']
  async ngOnInit(){
    this.motionService.isCirclesHidden.set(false)
    this.motionService.events.set(MotionEvent.RoutedToMain)
    const response = await this.service.search(1, 12);
    this.quizzes = response.quizzes;
    this.pageInfo = response.pageInfo;
  }
  async nextPage(){
    if(!this.pageInfo || !this.pageInfo.hasNextPage) return;
    const response = await this.service.search(this.pageInfo.currentPage + 1, 12);
    this.quizzes = response.quizzes;
    this.pageInfo = response.pageInfo;
  }
  async prevPage(){
    if(!this.pageInfo || !this.pageInfo.hasPrevPage) return;
    const response = await this.service.search(this.pageInfo.currentPage - 1, 12);
    this.quizzes = response.quizzes;
    this.pageInfo = response.pageInfo;
  }

  async play(quizId: string) {
    const result = await this.lobbyService.create(quizId);
    if(result.succeeded)
      await this.router.navigate(['lobby/'+ result.operationId]);
  }
  async delete(quizId: string){
    const result = await this.service.delete(quizId);
    if(result.succeeded)
      await this.ngOnInit()
  }
}

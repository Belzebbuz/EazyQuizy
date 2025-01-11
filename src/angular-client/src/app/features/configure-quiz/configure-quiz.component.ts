import {Component, inject, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {NgIf} from '@angular/common';
import {QuizInfoComponent} from './quiz-info/quiz-info.component';
import {MotionService} from '../../core/services/motion.service';
import {SelectQuestionComponent} from './select-question/select-question.component';
import {QuestionShortInfo, QuizInfo} from '../../../generated/types/types';
import {QuizService} from '../../core/services/quiz.service';
import {TypewriterComponent} from '../../core/motions/typewriter/typewriter.component';

@Component({
  selector: 'app-configure-quiz',
  imports: [
    NgIf,
    QuizInfoComponent,
    SelectQuestionComponent,
    TypewriterComponent
  ],
  templateUrl: './configure-quiz.component.html',
})
export class ConfigureQuizComponent implements OnInit{
  quizId!: string;
  motionService = inject(MotionService)
  service = inject(QuizService);
  quiz?: QuizInfo;
  questions: QuestionShortInfo[] = [];
  bgTexts: string[] = []
  isLoading = false;
  constructor(private route: ActivatedRoute) {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.quizId = id;
    this.updateQuiz = this.updateQuiz.bind(this);
  }
  async ngOnInit() {
    this.bgTexts = ['Загрузка...', 'Loading...']
    this.motionService.isCirclesHidden.set(true)
    await this.updateQuiz();
  }

  async updateQuiz() {
    this.isLoading = true;
    const response = await this.service.getInfo(this.quizId);
    if(!response.quiz) return;
    this.quiz = response.quiz;
    this.questions = response.questions;
    if(!this.quiz) return;
    this.quizId = this.quiz.id;
    this.bgTexts = [this.quiz.name, 'easy quiz']
    this.isLoading = false;
  }
}

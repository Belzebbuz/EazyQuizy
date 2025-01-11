import {Component, inject, OnInit} from '@angular/core';
import {SafeUrl} from '@angular/platform-browser';
import {FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {AddSingleQuestionRequest, SingleQuestionInfo} from '../../../../generated/quiz/quiz';
import {QuizService} from '../../../core/services/quiz.service';
import {InputTextareaComponent} from '../../../core/inputs/input-textarea/input-textarea.component';
import {InputTextArrayComponent} from '../../../core/inputs/input-text-array/input-text-array.component';
import {NgIf} from '@angular/common';
import {ContentStaggerDirective} from '../../../core/motions/directives/content-stagger.directive';
import {InputImageComponent} from '../../../core/inputs/input-image/input-image.component';
import {SlideTransitionDirective} from '../../../core/motions/directives/slide-transition.directive';
import {CpuComponent} from '../../../core/icons/cpu/cpu.component';

@Component({
  selector: 'app-configure-single-answer-question',
  imports: [
    ReactiveFormsModule,
    InputTextareaComponent,
    InputTextareaComponent,
    InputTextArrayComponent,
    NgIf,
    ContentStaggerDirective,
    InputImageComponent,
    SlideTransitionDirective,
    CpuComponent
  ],
  templateUrl: './configure-single-answer-question.component.html',
})
export class ConfigureSingleAnswerQuestionComponent implements OnInit {
  imageUrl?: SafeUrl;
  service = inject(QuizService);
  router = inject(Router);
  form = createSingleQuestionForm;
  quizId!: string;
  questionId?: string | null;
  isLoading = false;
  isWaitingGenerating = false;
  serverError?: string;
  answerControlFactory = defaultAnswerControl;
  constructor(private route: ActivatedRoute) {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.quizId = id;
    this.questionId = this.route.snapshot.paramMap.get('questionId');
  }

  async ngOnInit() {
    this.form.reset();
    if (this.questionId) {
      const info = await this.service.getQuestionInfo(this.quizId, this.questionId);
      if(!info.singleQuestionInfo) return;
      this.form.controls.text.setValue(info.singleQuestionInfo.text)
      this.form.controls.imageUrl.setValue(info.singleQuestionInfo.imageUrl)
      this.ensureFormArrayLength(this.form.controls.wrongAnswers, info.singleQuestionInfo.wrongAnswers.length, () => defaultAnswerControl());
      this.form.controls.correctAnswer.setValue(info.singleQuestionInfo.correctAnswer)
      this.form.controls.wrongAnswers.setValue(info.singleQuestionInfo.wrongAnswers)
    }
  }
  ensureFormArrayLength(formArray: FormArray, requiredLength: number, createControl: () => FormControl) {
    while (formArray.length < requiredLength) {
      formArray.push(createControl());
    }
  }
  get canGenerate() {
    return this.form.controls.text.valid && this.form.controls.wrongAnswers.invalid  && (!this.isWaitingGenerating || !this.isLoading);
  }
  async submit() {
    if(!this.form.valid || this.isLoading) return;
    this.isLoading = true;
    const request = AddSingleQuestionRequest.create();
    request.info = SingleQuestionInfo.create();
    request.info.questionId = this.questionId ?? undefined;
    request.quizId = this.quizId;
    request.info.correctAnswer = this.form.controls.correctAnswer.value!;
    request.info.wrongAnswers = this.form.controls.wrongAnswers.value.map(x => x).filter(x => x != null)
    request.info.text = this.form.controls.text.value!;
    request.info.imageUrl = this.form.controls.imageUrl.value ?? undefined;
    const result = await this.service.addSingleQuestion(request);
    this.isLoading = false;
    this.serverError = result.message;
    if(!result.succeeded) return;
    await this.routeToConfigure()
  }
  async routeToConfigure(){
    await this.router.navigate(['/configure/' + this.quizId]);
  }

  async generateAllAnswers() {
    if(!this.canGenerate) return;
    this.isWaitingGenerating = true;
    const invalidAnswers = this.form.controls.wrongAnswers.controls.filter(x => x.invalid);
    const count = invalidAnswers.length;
    const text = this.form.controls.text.value!;
    const result = await this.service.generateWrongAnswers(text, count);
    this.serverError = result.error
    if(!result.success) return;
    invalidAnswers.forEach(((f, index) => {
      f.setValue(result.answers[index])
    }))
    this.isWaitingGenerating = false;
  }
}
const defaultAnswerControl = () =>  new FormControl('', [Validators.required, Validators.maxLength(150)]);
const createSingleQuestionForm = new FormGroup({
  text:  new FormControl('', [Validators.required,  Validators.maxLength(250)]),
  correctAnswer: defaultAnswerControl(),
  imageUrl: new FormControl<string | undefined>(undefined),
  wrongAnswers: new FormArray([
    defaultAnswerControl(),
    defaultAnswerControl(),
    defaultAnswerControl(),
  ])
})


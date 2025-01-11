import {Component, inject, OnInit} from '@angular/core';
import {SafeUrl} from '@angular/platform-browser';
import {QuizService} from '../../../core/services/quiz.service';
import {ActivatedRoute, Router} from '@angular/router';
import {FormArray, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {
  AddMultipleQuestionRequest,
  AddSingleQuestionRequest,
  MultipleQuestionInfo
} from '../../../../generated/quiz/quiz';
import {ContentStaggerDirective} from '../../../core/motions/directives/content-stagger.directive';
import {CpuComponent} from '../../../core/icons/cpu/cpu.component';
import {InputImageComponent} from '../../../core/inputs/input-image/input-image.component';
import {InputTextArrayComponent} from '../../../core/inputs/input-text-array/input-text-array.component';
import {InputTextareaComponent} from '../../../core/inputs/input-textarea/input-textarea.component';
import {NgIf} from '@angular/common';
import {SlideTransitionDirective} from '../../../core/motions/directives/slide-transition.directive';

@Component({
  selector: 'app-configure-multiple-answer-question',
  imports: [
    ContentStaggerDirective,
    CpuComponent,
    FormsModule,
    InputImageComponent,
    InputTextArrayComponent,
    InputTextareaComponent,
    NgIf,
    SlideTransitionDirective,
    ReactiveFormsModule
  ],
  templateUrl: './configure-multiple-answer-question.component.html',
})
export class ConfigureMultipleAnswerQuestionComponent implements OnInit {
  imageUrl?: SafeUrl;
  service = inject(QuizService);
  router = inject(Router);
  form = createMultipleQuestionForm;
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
    this.form.reset()
    if (this.questionId) {
      const info = await this.service.getQuestionInfo(this.quizId, this.questionId);
      if(!info.multipleQuestionInfo) return;
      this.form.controls.text.setValue(info.multipleQuestionInfo.text)
      this.form.controls.imageUrl.setValue(info.multipleQuestionInfo.imageUrl)
      this.ensureFormArrayLength(this.form.controls.correctAnswers, info.multipleQuestionInfo.correctAnswers.length, () => defaultAnswerControl());
      this.ensureFormArrayLength(this.form.controls.wrongAnswers, info.multipleQuestionInfo.wrongAnswers.length, () => defaultAnswerControl());
      this.form.controls.correctAnswers.setValue(info.multipleQuestionInfo.correctAnswers)
      this.form.controls.wrongAnswers.setValue(info.multipleQuestionInfo.wrongAnswers)
    }
  }

  async submit() {
    if (!this.form.valid || this.isLoading) return;
    this.isLoading = true;
    const request = AddMultipleQuestionRequest.create();
    request.quizId = this.quizId;
    request.info = MultipleQuestionInfo.create();
    request.info.questionId = this.questionId ?? undefined;
    request.info.correctAnswers = this.form.controls.correctAnswers.value.map(x => x).filter(x => x != null);
    request.info.wrongAnswers = this.form.controls.wrongAnswers.value.map(x => x).filter(x => x != null)
    request.info.text = this.form.controls.text.value!;
    request.info.imageUrl = this.form.controls.imageUrl.value ?? undefined;
    const result = await this.service.addMultipleQuestion(request);
    this.isLoading = false;
    this.serverError = result.message;
    if (!result.succeeded) return;
    await this.router.navigate(['/configure/' + this.quizId]);
  }

  get canGenerate() {
    return this.form.controls.text.valid && this.form.controls.wrongAnswers.invalid && (!this.isWaitingGenerating || !this.isLoading);
  }

  async generateAllAnswers() {
    if (!this.canGenerate) return;
    this.isWaitingGenerating = true;
    const invalidAnswers = this.form.controls.wrongAnswers.controls.filter(x => x.invalid);
    const count = invalidAnswers.length;
    const text = this.form.controls.text.value!;
    const result = await this.service.generateWrongAnswers(text, count);
    this.serverError = result.error
    if (!result.success) return;
    invalidAnswers.forEach(((f, index) => {
      f.setValue(result.answers[index])
    }))
    this.isWaitingGenerating = false;
  }

  ensureFormArrayLength(formArray: FormArray, requiredLength: number, createControl: () => FormControl) {
    while (formArray.length < requiredLength) {
      formArray.push(createControl());
    }
  }
}

const defaultAnswerControl = (defaultValue: string = '') =>
  new FormControl(defaultValue, [Validators.required, Validators.maxLength(150)]);
const createMultipleQuestionForm = new FormGroup({
  text: new FormControl('', [Validators.required, Validators.maxLength(250)]),
  correctAnswers: new FormArray([
    defaultAnswerControl(),
    defaultAnswerControl(),
  ]),
  imageUrl: new FormControl<string | undefined>(undefined),
  wrongAnswers: new FormArray([
    defaultAnswerControl(),
    defaultAnswerControl(),
    defaultAnswerControl(),
  ])
})

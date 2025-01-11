import {Component, inject, OnInit} from '@angular/core';
import {FormArray, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {SafeUrl} from '@angular/platform-browser';
import {QuizService} from '../../../core/services/quiz.service';
import {ActivatedRoute, Router} from '@angular/router';
import {AddOrderQuestionRequest, OrderQuestionInfo} from '../../../../generated/quiz/quiz';
import {OrderedValue} from '../../../../generated/types/types';
import {ContentStaggerDirective} from '../../../core/motions/directives/content-stagger.directive';
import {InputImageComponent} from '../../../core/inputs/input-image/input-image.component';
import {InputTextArrayComponent} from '../../../core/inputs/input-text-array/input-text-array.component';
import {InputTextareaComponent} from '../../../core/inputs/input-textarea/input-textarea.component';
import {NgIf} from '@angular/common';
import {SlideTransitionDirective} from '../../../core/motions/directives/slide-transition.directive';

@Component({
  selector: 'app-configure-order-question',
  imports: [
    ContentStaggerDirective,
    FormsModule,
    InputImageComponent,
    InputTextArrayComponent,
    InputTextareaComponent,
    NgIf,
    SlideTransitionDirective,
    ReactiveFormsModule
  ],
  templateUrl: './configure-order-question.component.html',
})
export class ConfigureOrderQuestionComponent implements OnInit {
  imageUrl?: SafeUrl;
  service = inject(QuizService);
  router = inject(Router);
  form = createSingleQuestionForm;
  quizId!: string;
  questionId?: string | null;
  isLoading = false;
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
      if(!info.orderQuestionInfo) return;
      this.form.controls.text.setValue(info.orderQuestionInfo.text)
      this.form.controls.imageUrl.setValue(info.orderQuestionInfo.imageUrl)
      this.ensureFormArrayLength(this.form.controls.values, info.orderQuestionInfo.values.length, () => defaultAnswerControl());
      this.form.controls.values.setValue(info.orderQuestionInfo.values.map(x => x.value))
    }
  }
  ensureFormArrayLength(formArray: FormArray, requiredLength: number, createControl: () => FormControl) {
    while (formArray.length < requiredLength) {
      formArray.push(createControl());
    }
  }
  async submit() {
    if (!this.form.valid || this.isLoading) return;
    this.isLoading = true;
    const request = AddOrderQuestionRequest.create();
    request.quizId = this.quizId;
    request.info = OrderQuestionInfo.create();
    request.info.questionId = this.questionId ?? undefined;
    request.info.values = this.form.controls.values.value
      .filter(x => x != null)
      .map((x, index) => {
        const value = OrderedValue.create();
        value.value = x
        value.order = index
        return value;
      });
    request.info.text = this.form.controls.text.value!;
    request.info.imageUrl = this.form.controls.imageUrl.value ?? undefined;
    const result = await this.service.addOrderQuestion(request);
    this.isLoading = false;
    this.serverError = result.message;
    if (!result.succeeded) return;
    await this.routeToConfigure()
  }

  async routeToConfigure() {
    await this.router.navigate(['/configure/' + this.quizId]);
  }
}

const defaultAnswerControl = () => new FormControl('', [Validators.required, Validators.maxLength(150)]);
const createSingleQuestionForm = new FormGroup({
  text: new FormControl('', [Validators.required, Validators.maxLength(250)]),
  imageUrl: new FormControl<string | undefined>(undefined),
  values: new FormArray([
    defaultAnswerControl(),
    defaultAnswerControl(),
    defaultAnswerControl(),
  ])
})

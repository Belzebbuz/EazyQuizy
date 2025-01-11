import {Component, inject, OnDestroy, OnInit} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {SafeUrl} from '@angular/platform-browser';
import {QuizService} from '../../../core/services/quiz.service';
import {AddRangeQuestionRequest, RangeQuestionInfo} from '../../../../generated/quiz/quiz';
import {ContentStaggerDirective} from '../../../core/motions/directives/content-stagger.directive';
import {InputImageComponent} from '../../../core/inputs/input-image/input-image.component';
import {InputTextareaComponent} from '../../../core/inputs/input-textarea/input-textarea.component';
import {NgIf} from '@angular/common';
import {SlideTransitionDirective} from '../../../core/motions/directives/slide-transition.directive';
import {InputNumberComponent} from '../../../core/inputs/input-number/input-number.component';
import {gsap} from 'gsap';
import { Subscription } from 'rxjs/internal/Subscription';

@Component({
  selector: 'app-configure-range-question',
  imports: [
    ContentStaggerDirective,
    FormsModule,
    InputImageComponent,
    NgIf,
    SlideTransitionDirective,
    ReactiveFormsModule,
    InputNumberComponent,
    InputTextareaComponent
  ],
  templateUrl: './configure-range-question.component.html',
})
export class ConfigureRangeQuestionComponent implements OnInit, OnDestroy {
  imageUrl?: SafeUrl;
  service = inject(QuizService);
  router = inject(Router);
  form = createRangeQuestionForm;
  quizId!: string;
  questionId?: string | null;
  isLoading = false;
  serverError?: string;
  subs = new Subscription();
  constructor(private route: ActivatedRoute) {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.quizId = id;
    this.questionId = this.route.snapshot.paramMap.get('questionId');
  }

  ngOnDestroy(): void {
        this.subs.unsubscribe();
    }

  async ngOnInit() {
    this.form.reset();
    this.subscribeForRanger();
    if (this.questionId) {
      const info = await this.service.getQuestionInfo(this.quizId, this.questionId);
      if(!info.rangeQuestionInfo) return;
      this.form.controls.text.setValue(info.rangeQuestionInfo.text)
      this.form.controls.imageUrl.setValue(info.rangeQuestionInfo.imageUrl)
      this.form.controls.correctValue.setValue(info.rangeQuestionInfo.correctValue)
      this.form.controls.maxValue.setValue(info.rangeQuestionInfo.maxValue)
      this.form.controls.minValue.setValue(info.rangeQuestionInfo.minValue)
    }
  }

  private subscribeForRanger() {
    const sub1 = this.form.controls.correctValue.valueChanges.subscribe(this.onValuesChanged.bind(this));
    const sub2 = this.form.controls.minValue.valueChanges.subscribe(this.onValuesChanged.bind(this));
    const sub3 = this.form.controls.maxValue.valueChanges.subscribe(this.onValuesChanged.bind(this));
    this.subs.add(sub1)
    this.subs.add(sub2)
    this.subs.add(sub3)
    this.initRanger()
  }

  get invalidState (){
    const minValue = this.form.controls.minValue.value;
    const maxValue = this.form.controls.maxValue.value;
    const correctValue = this.form.controls.correctValue.value;
    const invalidBorders = maxValue <= minValue;
    const invalidCorrectValue = correctValue < minValue || correctValue > maxValue;
    return invalidBorders || invalidCorrectValue;
  }
  onValuesChanged() {
    const ranger = document.querySelector<HTMLElement>(".ranger");
    const current = document.querySelector<HTMLElement>(".cur");
    if (!ranger || !current) return;
    const rangerRect = ranger.getBoundingClientRect();
    const minValue = this.form.controls.minValue.value;
    const maxValue = this.form.controls.maxValue.value;
    const correctValue = this.form.controls.correctValue.value;
    const invalidBorders = maxValue <= minValue;
    const invalidCorrectValue = correctValue < minValue || correctValue > maxValue;
    if (invalidBorders || invalidCorrectValue){
      gsap.to(ranger, {
        duration: 0.5,
        opacity: 0,
      })
      return;
    }
    const percent = (correctValue - minValue) / (maxValue - minValue)
    const newX = percent * rangerRect.width;
    gsap.to(ranger, {
      duration: 0.5,
      opacity: 1,
    })
    gsap.to(current, {
      duration: .5,
      opacity: 1,
      x: newX - (correctValue == maxValue ? 6 : 0),
      ease: "power1.inOut"
    })
  }

  initRanger() {
    const ranger = document.querySelector<HTMLElement>(".ranger");
    const max = document.querySelector<HTMLElement>(".max");
    const current = document.querySelector<HTMLElement>(".cur");
    if (!ranger || !max || !current) return;
    const rangerRect = ranger.getBoundingClientRect();
    gsap.to(ranger,{
      duration: 0,
      opacity: 0,
    })
    gsap.to(current, {
      duration: 0,
      opacity: 0
    })
    gsap.to(max, {
      duration: 1,
      x: rangerRect.width - 6
    })
    gsap.to(ranger,{
      duration: 1,
      opacity: 1,
    })
  }

  async submit() {
    if (!this.form.valid || this.isLoading) return;
    this.isLoading = true;
    const request = AddRangeQuestionRequest.create();
    request.quizId = this.quizId;
    request.info = RangeQuestionInfo.create();
    request.info.questionId = this.questionId ?? undefined;
    request.info.correctValue = this.form.controls.correctValue.value;
    request.info.minValue = this.form.controls.minValue.value;
    request.info.maxValue = this.form.controls.maxValue.value;
    request.info.text = this.form.controls.text.value!;
    request.info.imageUrl = this.form.controls.imageUrl.value ?? undefined;
    const result = await this.service.addRangeQuestion(request);
    this.isLoading = false;
    this.serverError = result.message;
    if (!result.succeeded) return;
    await this.router.navigate(['/configure/' + this.quizId]);
  }
}

const createRangeQuestionForm = new FormGroup({
  text: new FormControl('', [Validators.required, Validators.maxLength(250)]),
  correctValue: new FormControl<number>(0, {nonNullable: true}),
  minValue: new FormControl<number>(0, {nonNullable: true}),
  maxValue: new FormControl<number>(0, {nonNullable: true}),
  imageUrl: new FormControl<string | undefined>(undefined)
})

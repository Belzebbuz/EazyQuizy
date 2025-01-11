import {Component, inject, OnDestroy, OnInit} from '@angular/core';
import {QuizService} from '../../core/services/quiz.service';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {CreateQuizRequest} from '../../../generated/quiz/quiz';
import {NgIf} from '@angular/common';
import {Router} from '@angular/router';
import {InputTextComponent} from '../../core/inputs/input-text/input-text.component';
import {TooltipDirective} from '../../core/tooltip/tooltip.directive';
import {gsap} from 'gsap';
import {MotionEvent, MotionService} from '../../core/services/motion.service';
import {InputChipsComponent} from '../../core/inputs/input-chips/input-chips.component';
@Component({
  selector: 'app-create-quiz',
  templateUrl: './create-quiz.component.html',
  imports: [
    ReactiveFormsModule,
    FormsModule,
    NgIf,
    InputTextComponent,
    InputChipsComponent,
    TooltipDirective,
    InputChipsComponent
  ]
})
export class CreateQuizComponent implements OnDestroy, OnInit {
  service = inject(QuizService)
  router = inject(Router)
  motionService = inject(MotionService)
  form = createQuizForm;
  isLoading = false;
  serverError?:string;

  ngOnInit(): void {
    this.motionService.events.set(MotionEvent.RoutedToCreate)
    this.motionService.isCirclesHidden.set(false)
  }
  async submit(){
    if(!this.form.valid || this.isLoading)
      return;
    this.isLoading = true;
    const request = CreateQuizRequest.create();
    request.name = this.form.controls.name.value ?? 'DEFAULT_NAME';
    request.tags = this.form.controls.tags.value;
    const result = await this.service.create(request);
    this.isLoading = false;
    if(!result.succeeded)
      this.serverError = result.message;
    else{
      this.routeToConfigure(result.operationId)
    }
  }
  ngOnDestroy(): void {
    this.form.reset();
  }
  routeToConfigure(id: string) {
    const formElem = document.querySelector<HTMLElement>('#create-form');
    if(!formElem) return;
    this.motionService.events.set(MotionEvent.RoutedToConfigure)
    gsap.to(formElem,{
      duration: 0.4,
      y: -1000,
      ease: "back.inOut",
      onComplete: () => {
        this.router.navigate(['/configure/' + id]).then(r =>true);
      }
    })
  }
}

const createQuizForm = new FormGroup({
  name: new FormControl<string>('',[Validators.required, Validators.minLength(5), Validators.maxLength(50)]),
  tags: new FormControl<string[]>([], { nonNullable: true })
});

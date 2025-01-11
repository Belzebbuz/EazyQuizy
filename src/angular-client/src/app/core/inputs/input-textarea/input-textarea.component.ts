import {Component, Input} from '@angular/core';
import {FormGroup, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-input-textarea',
  imports: [
    FormsModule,
    NgIf,
    ReactiveFormsModule
  ],
  templateUrl: './input-textarea.component.html',
})
export class InputTextareaComponent {
  @Input() controlName?: string;
  @Input() form!: FormGroup;
  @Input() label?: string;
}

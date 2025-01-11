import {Component, Input} from '@angular/core';
import {FormGroup, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-input-text',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgIf
  ],
  templateUrl: './input-text.component.html',
})
export class InputTextComponent {
  @Input() controlName!: string;
  @Input() form!: FormGroup;
  @Input() label!: string;
}

import {Component, Input} from '@angular/core';
import {FormGroup, ReactiveFormsModule} from '@angular/forms';

@Component({
  selector: 'app-input-number',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './input-number.component.html',
})
export class InputNumberComponent {
  @Input() form!: FormGroup
  @Input() controlName!: string
  @Input() label!: string;
  add() {
    const currentValue = this.form.get(this.controlName)?.value as number;
    if(currentValue == null) return;
    this.form.get(this.controlName)?.setValue(currentValue + 1);
  }

  remove() {
    const currentValue = this.form.get(this.controlName)?.value as number;
    if(currentValue == null) return;
    this.form.get(this.controlName)?.setValue(currentValue - 1);
  }
}

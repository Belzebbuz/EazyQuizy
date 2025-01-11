import {Component, Input} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgForOf, NgIf} from '@angular/common';

@Component({
  selector: 'app-input-chips',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgForOf,
    NgIf
  ],
  templateUrl: './input-chips.component.html',
})
export class InputChipsComponent {
  @Input() controlName?: string;
  @Input() form!: FormGroup;
  @Input() label?: string;
  currentInputValue?: string;
  get controlArray() {
    if(!this.controlName)
      return;
    return  this.form.controls[this.controlName] as FormControl<string[]>;
  }
  addValue() {
    if(!this.currentInputValue || !this.controlArray)
      return;
    const currentItems = this.controlArray.value;
    this.controlArray.setValue([...currentItems, this.currentInputValue]);
    this.currentInputValue = undefined;
  }

  removeValue(tag: string) {
    if(!this.controlArray)
      return;
    const currentItems = this.controlArray.value;
    this.controlArray.setValue(currentItems.filter((value) => value !== tag));
  }
}

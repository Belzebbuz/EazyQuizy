import {Component, Input} from '@angular/core';
import {FormArray, FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {NgForOf, NgIf} from '@angular/common';

@Component({
  selector: 'app-input-text-array',
  imports: [
    NgIf,
    ReactiveFormsModule,
    NgForOf,
  ],
  templateUrl: './input-text-array.component.html',
})
export class InputTextArrayComponent {
  @Input() array!: FormArray<FormControl<string | null>>;
  @Input() form!: FormGroup;
  @Input() label!: string;
  @Input() controlFactory!: () => FormControl;
  @Input() minCount = 3;
  @Input() maxCount = 5;
  get canAddWrongAnswer() {
    return this.array.length < this.maxCount;
  }
  get canRemoveWrongAnswer() {
    return this.array.length > this.minCount;
  }

  add(){
    if(!this.canAddWrongAnswer) return;
    this.array.insert(this.array.length,this.controlFactory())
  }
  remove(index: number){
    if(!this.canRemoveWrongAnswer) return;
    this.array.removeAt(index);
  }
}

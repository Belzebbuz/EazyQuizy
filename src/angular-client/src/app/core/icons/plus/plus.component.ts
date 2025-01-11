import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-plus',
  imports: [],
  templateUrl: './plus.component.html',
})
export class PlusComponent {
  @Input() className?: string;
}

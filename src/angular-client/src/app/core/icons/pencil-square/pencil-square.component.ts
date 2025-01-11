import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-pencil-square',
  imports: [],
  templateUrl: './pencil-square.component.html',
})
export class PencilSquareComponent {
  @Input() className: string = 'w-6 h-6';
}

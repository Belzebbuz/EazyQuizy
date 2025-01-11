import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-trash',
  imports: [],
  templateUrl: './trash.component.html',
})
export class TrashComponent {
  @Input() className: string = 'w-6 h-6';
}

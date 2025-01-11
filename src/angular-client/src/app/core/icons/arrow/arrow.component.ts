import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-arrow',
  imports: [],
  templateUrl: './arrow.component.html'
})
export class ArrowComponent {
  @Input() className = "w-6 h-6";
}

import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-logout-arrow',
  imports: [],
  templateUrl: './logout-arrow.component.html',
})
export class LogoutArrowComponent {
  @Input() className: string = 'w-6 h-6';
}

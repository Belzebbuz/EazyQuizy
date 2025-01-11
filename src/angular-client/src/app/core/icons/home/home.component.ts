import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.component.html',
})
export class HomeComponent {
  @Input() className: string = 'w-6 h-6';
}

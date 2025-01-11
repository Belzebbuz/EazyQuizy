import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-moon-stars-fill',
  imports: [],
  templateUrl: './moon-stars-fill.component.html',
})
export class MoonStarsFillComponent {
  @Input() className?: string;
}

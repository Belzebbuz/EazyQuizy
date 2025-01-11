import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-arrows-pointing-out',
  imports: [],
  templateUrl: './arrows-pointing-out.component.html',
})
export class ArrowsPointingOutComponent {
  @Input() className: string = 'w-6 h-6';
}

import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-magnifying-glass',
  imports: [],
  templateUrl: './magnifying-glass.component.html',
})
export class MagnifyingGlassComponent {
  @Input() className?: string;
}

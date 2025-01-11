import {Component, inject} from '@angular/core';
import {MotionService} from '../../services/motion.service';
import {BreakpointObserver, Breakpoints} from '@angular/cdk/layout';
import {MagneticCircleComponent} from '../magnetic-circle/magnetic-circle.component';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-magnetic-background',
  imports: [
    MagneticCircleComponent,
    NgIf
  ],
  templateUrl: './magnetic-background.component.html',
})
export class MagneticBackgroundComponent {
  motion = inject(MotionService);
  isSmallScreen: boolean = false;
  breakpointObserver = inject(BreakpointObserver);
  constructor() {
    this.breakpointObserver
      .observe([Breakpoints.Handset])
      .subscribe((result) => {
        this.isSmallScreen = result.matches;
      });
  }
}

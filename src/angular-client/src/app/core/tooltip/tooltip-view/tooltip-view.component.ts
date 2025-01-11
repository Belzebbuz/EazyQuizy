import {AfterViewInit, Component, ElementRef, inject, Input, Renderer2} from '@angular/core';
import {animate, AnimationBuilder, AnimationPlayer, style, transition, trigger} from '@angular/animations';

@Component({
  selector: 'app-tooltip-view',

  templateUrl: './tooltip-view.component.html',
  animations: [
    trigger('fade', [
      transition(':enter', [
        style(
          {
            opacity: 0,
            filter: 'blur(5px)'
          }),
        animate('300ms 100ms', style({ opacity: 1, filter: 'blur(0)' }))
      ])
    ])
  ]
})
export class TooltipViewComponent implements AfterViewInit {
  @Input() text: string = '';
  @Input() parentPos = {top: 0, left: 0, width: 0, height: 0}
  elementRef = inject(ElementRef)
  renderer = inject(Renderer2)
  animationBuilder = inject(AnimationBuilder)
  ngAfterViewInit(): void {
    const tooltipRect = this.elementRef.nativeElement.getBoundingClientRect();
    const centerX = (this.parentPos.width / 2) - (tooltipRect.width / 2) + this.parentPos.left;
    const centerY = this.parentPos.top + (this.parentPos.height / 2) - (tooltipRect.height / 2) + 64;
    const adjustedPosition = {
      top: `${centerY}px`,
      left: `${centerX}px`,
    };
    this.renderer.setStyle(this.elementRef.nativeElement, 'top', adjustedPosition.top);
    this.renderer.setStyle(this.elementRef.nativeElement, 'left', adjustedPosition.left);
  }
  public fadeOut() {
    const animation = this.animationBuilder.build([
      animate('200ms', style({ opacity: 0, filter: 'blur(2px)' }))
    ]);

    const player = animation.create(this.elementRef.nativeElement);
    player.play();

    player.onDone(() => {
      this.elementRef.nativeElement.remove();
    });
  }
}

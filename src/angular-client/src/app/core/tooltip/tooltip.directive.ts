import {
  ComponentRef,
  Directive,
  ElementRef,
  HostListener,
  inject,
  Input,
  Renderer2,
  ViewContainerRef
} from '@angular/core';
import {TooltipViewComponent} from './tooltip-view/tooltip-view.component';

@Directive({
  selector: '[appTooltip]'
})
export class TooltipDirective {
  @Input('appTooltip') tooltipText: string = '';

  private tooltipRef: ComponentRef<TooltipViewComponent> | null = null;
  containerRef = inject(ViewContainerRef)
  elementRef = inject(ElementRef)
  renderer = inject(Renderer2)
  private hideTimeout: any;

  @HostListener('mouseenter')
  onMouseEnter() {
    if (this.hideTimeout) {
      clearTimeout(this.hideTimeout);
      this.hideTimeout = null;
      return;
    }
    if (!this.tooltipText) return;

    this.tooltipRef = this.containerRef.createComponent(TooltipViewComponent);
    const hostRect = this.elementRef.nativeElement.getBoundingClientRect();
    this.tooltipRef.instance.text = this.tooltipText;
    this.tooltipRef.instance.parentPos = {
      top: hostRect.top,
      left: hostRect.left,
      width: hostRect.width,
      height: hostRect.height
    };
    this.renderer.setStyle(this.tooltipRef.location.nativeElement, 'position', 'absolute');
    this.renderer.setStyle(this.tooltipRef.location.nativeElement, 'z-index', '999');
  }

  @HostListener('mouseleave')
  onMouseLeave() {
    if (this.tooltipRef) {
      this.hideTimeout = setTimeout(() => {
        this.tooltipRef?.instance.fadeOut();
        this.tooltipRef = null;
        this.hideTimeout = null;
      }, 1000);
    }
  }
}

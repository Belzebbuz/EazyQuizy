import {AfterViewInit, Directive, ElementRef, inject, Input} from '@angular/core';
import {gsap} from 'gsap';

@Directive({
  selector: '[appSlideTransition]'
})
export class SlideTransitionDirective implements AfterViewInit{
  elementRef = inject(ElementRef)
  @Input() direction: SlideDirection = 'up'
    ngAfterViewInit(): void {
      gsap.to(this.elementRef.nativeElement, {
        duration: 0,
        opacity: 0,
        y: 100
      })
      gsap.to(this.elementRef.nativeElement, {
        duration: 1,
        opacity: 1,
        y: 0,
        ease: "power4.out"
      })
    }


}
export type SlideDirection = 'up' | 'down'

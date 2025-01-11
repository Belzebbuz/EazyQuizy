import {AfterViewInit, Directive, ElementRef, inject, Input} from '@angular/core';
import {gsap} from 'gsap/all';

@Directive({
  selector: '[appContentStagger]'
})
export class ContentStaggerDirective implements AfterViewInit {
  elementRef = inject(ElementRef)
  @Input() direction: StaggerDirection = 'left'
  ngAfterViewInit(): void {
    const children = this.elementRef.nativeElement.children as HTMLCollection
    const initX = this.direction == 'left' ? 100 : -100;
    gsap.fromTo(
      Array.from(children),
      {
        opacity: 0,
        x: initX,
      },
      {
        opacity: 1,
        x: 0,
        duration: 1,
        stagger: 0.1,
        onComplete: () => {
          gsap.set(Array.from(children), { clearProps: "transform,opacity" });
        }
      }
    );
  }
}
export type StaggerDirection = 'left' | 'right'

import {AfterViewInit, Component, inject, Input} from '@angular/core';
import {Elastic, gsap, Power4} from 'gsap/all';
import {NgStyle} from '@angular/common';
import {MotionEvent, MotionService} from '../../services/motion.service';
import {G} from '@angular/cdk/keycodes';

@Component({
  selector: 'app-magnetic-circle',
  imports: [
    NgStyle
  ],
  templateUrl: './magnetic-circle.component.html'
})
export class MagneticCircleComponent implements AfterViewInit{
    @Input() size = 8;
    outerId = "id" + crypto.randomUUID();
    innerId = "id" + crypto.randomUUID();
    service = inject(MotionService)
    floatingTimeline?: any;
    floatStrength = 20;
    ngAfterViewInit(): void {
      this.initEventListener();
      this.initMagnetic();
      this.initRandomFloating();
    }

  private initRandomFloating() {
    const outer = document.querySelector<HTMLElement>('#' + this.outerId)
    if (!outer)
      return;
    this.floatingTimeline = gsap.timeline({
      repeat: -1,
      yoyo: true
    })
    this.floatingTimeline.to(outer, {
      x: this.randomFloat() * this.floatStrength,
      y: this.randomFloat() * this.floatStrength,
      duration: this.randomFloat(2, 6),
      ease: "power1.inOut",
      onComplete: this.initRandomFloating
    })
  }

  private initEventListener() {
    const inner = document.querySelector<HTMLElement>('#' + this.innerId)
    const outer = document.querySelector<HTMLElement>('#' + this.outerId)
    if(!outer) return;
    this.service.observable.subscribe(event => {
      if (event == MotionEvent.RoutedToConfigure) {
        gsap.to(inner, {
          duration: .7,
          opacity: 0,
          ease: Power4.easeOut
        })
      }
      if (event == MotionEvent.RoutedToCreate) {
        gsap.to(inner, {
          duration: .7,
          zoom: 1.4,
          opacity: 1,
          ease: Power4.easeOut,
          onComplete: () => {
            this.floatStrength = 40
          }
        })
      }
      if (event == MotionEvent.RoutedToMain) {
        gsap.to(inner, {
          duration: 1,
          zoom: 1,
          opacity: 1,
          ease: Power4.easeOut,
          onComplete: () => {
            this.floatStrength = 20
          }
        })
      }
    })
  }

  randomFloat(min = -1, max = 1) {
    return Math.random() * (max - min) + min;
  }

  private initMagnetic() {
    const outer = document.querySelector<HTMLElement>('#' + this.outerId)
    const strength = 70;

    if (!outer)
      return;
    const activateMagneto = (event: any) => {
      let boundBox = outer.getBoundingClientRect();
      const newX = ((event.clientX - boundBox.left) / outer.offsetWidth) - 0.5;
      const newY = ((event.clientY - boundBox.top) / outer.offsetHeight) - 0.5;
      this.floatingTimeline?.pause()

      gsap.to(outer, {
        duration: 1,
        x: newX * strength,
        y: newY * strength,
        ease: Power4.easeOut
      })
    }
    const resetMagneto = (event: Event) => {
      gsap.to(outer, {
        duration: 2,
        x: 0,
        y: 0,
        ease: Elastic.easeOut,
        onComplete: () => {
          this.floatingTimeline?.restart()
        }
      })

    }
    outer.addEventListener('mousemove', activateMagneto)
    outer.addEventListener('mouseleave', resetMagneto)
  }
}

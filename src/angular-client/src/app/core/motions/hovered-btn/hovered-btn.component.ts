import {AfterViewInit, Component, Input, OnInit} from '@angular/core';
import {gsap} from 'gsap';

@Component({
  selector: 'app-hovered-btn',
  imports: [],
  templateUrl: './hovered-btn.component.html',
})
export class HoveredBtnComponent implements AfterViewInit{
    @Input() text?: string;
    buttonId = 'id' + crypto.randomUUID();
    hoverId = 'id' + crypto.randomUUID();
    ngAfterViewInit(): void {
      const button = document.querySelector<HTMLElement>('#' + this.buttonId);
      const hover = document.querySelector<HTMLElement>('#' + this.hoverId);
      if(!button || !hover) return;
      const testButtonEnter = (event: Event) => {
        gsap.to(hover, {
          duration: .5,
          height: '150%'
        })
        gsap.to(button, {
          duration: .5,
          color: '#e5e7eb'
        })
      }
      const testButtonLeave = (event: Event) => {
        gsap.to(hover, {
          duration: .5,
          height: '0%'
        })
        gsap.to(button, {
          duration: .5,
          color: '#56828c',
          onComplete: () => {
            gsap.set(button, { clearProps: "color" });
          }
        })
      }
      button.addEventListener('mouseenter', testButtonEnter)
      button.addEventListener('mouseleave', testButtonLeave)
    }

}

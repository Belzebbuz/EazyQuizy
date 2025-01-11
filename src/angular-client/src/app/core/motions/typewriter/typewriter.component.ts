import {AfterViewInit, Component, Input, OnInit} from '@angular/core';
import {gsap} from 'gsap';

@Component({
  selector: 'app-typewriter',
  imports: [],
  templateUrl: './typewriter.component.html',
})
export class TypewriterComponent implements AfterViewInit{
  @Input() texts: string[] = [];
  idTypewriter = 'id' + crypto.randomUUID().replaceAll('-','');
  idCursor = 'id' + crypto.randomUUID().replaceAll('-','');
  ngAfterViewInit(): void {
    const typewriterId = '#' + this.idTypewriter;
    let mainTimeLine = gsap.timeline({
      repeat: -1
    })
    this.texts.forEach(word =>{
      let textTimeLine = gsap.timeline({
        repeat: 1,
        yoyo: true,
        repeatDelay: 4
      })
      textTimeLine.to(typewriterId, {
        text: word,
        duration: 1,
        onUpdate: () => {
          cursorTimeline.restart();
          cursorTimeline.pause();
        },
        onComplete: () => {
          cursorTimeline.play()
        }
      })
      mainTimeLine.add(textTimeLine);
    })
    let cursorTimeline = gsap.timeline({
      repeat: -1,
      repeatDelay: .8
    })
    cursorTimeline.to('#' + this.idCursor, {
      opacity: 1,
      duration: 0
    }).to('#' + this.idCursor, {
      opacity: 0,
      duration: 0,
      delay: .8
    })
  }
}

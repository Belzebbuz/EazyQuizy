import {Injectable, signal} from '@angular/core';
import {toObservable} from "@angular/core/rxjs-interop";

@Injectable({
  providedIn: 'root'
})
export class MotionService {
  public events = signal<MotionEvent>(MotionEvent.NoEvent);
  public observable = toObservable(this.events);

  public isCirclesHidden = signal(true);
}
export enum MotionEvent {
  NoEvent,
  RoutedToCreate,
  RoutedToMain,
  RoutedToConfigure
}

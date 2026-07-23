import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TitleService {
  private titleSubject = new BehaviorSubject<string>('Dashboard');
  public title$: Observable<string> = this.titleSubject.asObservable();

  setTitle(newTitle: string): void {
    this.titleSubject.next(newTitle);
  }
}

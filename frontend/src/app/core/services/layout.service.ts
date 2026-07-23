import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LayoutService {
  private sidebarOpenSubject = new BehaviorSubject<boolean>(false);
  public isSidebarOpen$: Observable<boolean> = this.sidebarOpenSubject.asObservable();

  constructor() {}

  toggleSidebar(): void {
    this.sidebarOpenSubject.next(!this.sidebarOpenSubject.value);
  }

  setSidebarOpen(isOpen: boolean): void {
    this.sidebarOpenSubject.next(isOpen);
  }
}

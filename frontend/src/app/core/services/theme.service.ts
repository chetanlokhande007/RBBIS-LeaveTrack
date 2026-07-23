import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private activeThemeSubject = new BehaviorSubject<'light' | 'dark'>('light');
  public activeTheme$: Observable<'light' | 'dark'> = this.activeThemeSubject.asObservable();

  constructor() {
    const savedTheme = localStorage.getItem('leavetrack-theme') as 'light' | 'dark' | null;
    if (savedTheme) {
      this.setTheme(savedTheme);
    } else {
      const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
      this.setTheme(prefersDark ? 'dark' : 'light');
    }
  }

  get currentTheme(): 'light' | 'dark' {
    return this.activeThemeSubject.value;
  }

  toggleTheme(): void {
    const nextTheme = this.currentTheme === 'light' ? 'dark' : 'light';
    this.setTheme(nextTheme);
  }

  setTheme(theme: 'light' | 'dark'): void {
    this.activeThemeSubject.next(theme);
    localStorage.setItem('leavetrack-theme', theme);
    if (theme === 'dark') {
      document.documentElement.setAttribute('data-theme', 'dark');
    } else {
      document.documentElement.removeAttribute('data-theme');
    }
  }
}

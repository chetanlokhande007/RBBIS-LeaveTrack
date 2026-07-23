import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  show(message: string, type: 'success' | 'error' | 'info' = 'success'): void {
    const container = document.getElementById('toast-container');
    if (!container) return;

    const toastEl = document.createElement('div');
    toastEl.className = 'toast';
    
    const checkmark = type === 'success' ? '<span class="tick">✓</span>' : '';
    toastEl.innerHTML = `${checkmark}<span>${message}</span>`;
    
    container.appendChild(toastEl);

    setTimeout(() => {
      toastEl.style.opacity = '0';
      toastEl.style.transition = 'opacity .3s';
      setTimeout(() => toastEl.remove(), 300);
    }, 2600);
  }
}

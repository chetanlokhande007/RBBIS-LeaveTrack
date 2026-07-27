import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HeaderComponent } from '../header/header.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink, HeaderComponent, FooterComponent],
  template: `
    <div class="page-wrapper">
      <app-header></app-header>
      <div class="not-found-container">
        <h1>404</h1>
        <h2>Page Not Found</h2>
        <p>The page you are looking for doesn't exist or has been moved.</p>
        <a routerLink="/" class="btn btn-primary">Go to Homepage</a>
      </div>
      <app-footer></app-footer>
    </div>
  `,
  styles: [`
    .not-found-container {
      flex: 1;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      text-align: center;
      padding: 4rem 2rem;
      min-height: 60vh;
    }
    h1 {
      font-size: 6rem;
      color: #2563eb;
      margin: 0;
      line-height: 1;
    }
    h2 {
      font-size: 2rem;
      color: #0f172a;
      margin: 1rem 0;
    }
    p {
      color: #475569;
      margin-bottom: 2rem;
      max-width: 400px;
    }
    .btn {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      padding: 0.625rem 1.25rem;
      border-radius: 6px;
      font-weight: 500;
      font-size: 0.875rem;
      transition: all 0.2s ease;
      cursor: pointer;
      text-decoration: none;
      background-color: #7b61ff;
      color: white;
      border: 1px solid #7b61ff;
    }
    .btn:hover {
      background-color: #6a4bf7;
    }
  `]
})
export class NotFoundComponent { }

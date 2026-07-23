import { Component, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { TitleService } from '../../core/services/title.service';
import { ThemeService } from '../../core/services/theme.service';
import { LayoutService } from '../../core/services/layout.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './topbar.component.html'
})
export class TopbarComponent implements OnInit {
  pageTitle = 'Dashboard';
  bellOpen = false;
  userMenuOpen = false;

  constructor(
    public authService: AuthService,
    private titleService: TitleService,
    public themeService: ThemeService,
    public layoutService: LayoutService,
    private router: Router
  ) {}

  ngOnInit() {
    this.titleService.title$.subscribe(t => {
      this.pageTitle = t;
    });
  }

  get userInitials(): string {
    const name = this.authService.currentUserValue?.fullName || '';
    if (!name) return 'U';
    const parts = name.split(' ');
    return parts.map(p => p[0]).join('').substring(0, 2).toUpperCase();
  }

  get userRoleTitle(): string {
    const role = this.authService.userRole;
    if (role === 'Admin') return 'Admin / HR';
    return role || '';
  }

  get rolePillClass(): string {
    const role = this.authService.userRole;
    return `role-pill ${role?.toLowerCase() || ''}`;
  }

  toggleBell(event: Event) {
    event.stopPropagation();
    this.userMenuOpen = false;
    this.bellOpen = !this.bellOpen;
  }

  toggleUserMenu(event: Event) {
    event.stopPropagation();
    this.bellOpen = false;
    this.userMenuOpen = !this.userMenuOpen;
  }

  logout() {
    this.authService.logout();
    this.userMenuOpen = false;
  }

  @HostListener('document:click', ['$event'])
  closeMenus() {
    this.bellOpen = false;
    this.userMenuOpen = false;
  }
}

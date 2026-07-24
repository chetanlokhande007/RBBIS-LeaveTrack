import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ThemeService } from '../../core/services/theme.service';

import { LayoutService } from '../../core/services/layout.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html'
})
export class SidebarComponent {
  userDropdownOpen = false;

  constructor(
    public authService: AuthService, 
    public themeService: ThemeService,
    public layoutService: LayoutService,
    private router: Router
  ) {}

  get userInitials(): string {
    const name = this.authService.currentUserValue?.fullName || '';
    if (!name) return 'U';
    const parts = name.split(' ');
    return parts.map(p => p[0]).join('').substring(0, 2).toUpperCase();
  }

  get userFullName(): string {
    return this.authService.currentUserValue?.fullName || 'User';
  }

  get userRoleTitle(): string {
    const role = this.authService.userRole;
    if (role === 'Admin' || role === 'HR') return 'Admin / HR';
    return role || '';
  }

  get isAdmin(): boolean {
    return this.authService.userRole === 'Admin' || this.authService.userRole === 'HR';
  }

  get isManager(): boolean {
    return this.authService.userRole === 'Manager';
  }

  get dashboardRoute(): string {
    const role = this.authService.userRole;
    if (role === 'Admin') return '/admin/dashboard';
    if (role === 'HR') return '/admin/employees';
    if (role === 'Manager') return '/manager/dashboard';
    return '/employee/dashboard';
  }

  toggleDropdown(event: Event) {
    event.stopPropagation();
    this.userDropdownOpen = !this.userDropdownOpen;
  }

  logout() {
    this.authService.logout();
    this.userDropdownOpen = false;
  }
}

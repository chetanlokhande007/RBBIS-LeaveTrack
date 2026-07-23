import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const expectedRoles = route.data['roles'] as string[];
  const currentRole = authService.userRole;

  if (authService.isLoggedIn && currentRole && expectedRoles.includes(currentRole)) {
    return true;
  }

  if (currentRole === 'Admin') {
    router.navigate(['/admin/dashboard']);
  } else if (currentRole === 'Manager') {
    router.navigate(['/manager/dashboard']);
  } else if (currentRole === 'Employee') {
    router.navigate(['/employee/dashboard']);
  } else {
    router.navigate(['/login']);
  }
  
  return false;
};

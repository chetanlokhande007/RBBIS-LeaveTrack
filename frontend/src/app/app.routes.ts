import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login.component';
import { ShellComponent } from './layouts/shell/shell.component';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

// Employee Portal
import { EmployeeDashboardComponent } from './features/employee/dashboard.component';
import { ApplyLeaveComponent } from './features/employee/apply-leave.component';
import { MyRequestsComponent } from './features/employee/my-requests.component';
import { MyAttendanceComponent } from './features/employee/my-attendance.component';

// Manager Portal
import { ManagerDashboardComponent } from './features/manager/dashboard.component';
import { ApprovalsComponent } from './features/manager/approvals.component';

// Admin Portal
import { AdminDashboardComponent } from './features/admin/dashboard.component';
import { EmployeesComponent } from './features/admin/employees.component';
import { LeaveTypesComponent } from './features/admin/leave-types.component';
import { ReportsComponent } from './features/admin/reports.component';
import { AuditLogComponent } from './features/admin/audit-log.component';

import { ForgotPasswordComponent } from './features/auth/forgot-password.component';
import { ResetPasswordComponent } from './features/auth/reset-password.component';
import { HomeComponent } from './features/public/home.component';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  
  // Protected portal shell
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      // Employee routes
      {
        path: 'employee/dashboard',
        component: EmployeeDashboardComponent,
        canActivate: [roleGuard],
        data: { roles: ['Employee'] }
      },
      {
        path: 'employee/apply',
        component: ApplyLeaveComponent,
        canActivate: [roleGuard],
        data: { roles: ['Employee'] }
      },
      {
        path: 'employee/requests',
        component: MyRequestsComponent,
        canActivate: [roleGuard],
        data: { roles: ['Employee'] }
      },
      {
        path: 'employee/attendance',
        component: MyAttendanceComponent,
        canActivate: [roleGuard],
        data: { roles: ['Employee'] }
      },

      // Manager routes
      {
        path: 'manager/dashboard',
        component: ManagerDashboardComponent,
        canActivate: [roleGuard],
        data: { roles: ['Manager'] }
      },
      {
        path: 'manager/approvals',
        component: ApprovalsComponent,
        canActivate: [roleGuard],
        data: { roles: ['Manager'] }
      },

      // Admin routes
      {
        path: 'admin/dashboard',
        component: AdminDashboardComponent,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      },
      {
        path: 'admin/employees',
        component: EmployeesComponent,
        canActivate: [roleGuard],
        data: { roles: ['Admin', 'HR'] }
      },
      {
        path: 'admin/leave-types',
        component: LeaveTypesComponent,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      },
      {
        path: 'admin/reports',
        component: ReportsComponent,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      },
      {
        path: 'admin/audit',
        component: AuditLogComponent,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      },

      // Fallbacks
      { path: '', redirectTo: 'employee/dashboard', pathMatch: 'full' }
    ]
  },

  { path: '**', redirectTo: 'home' }
];

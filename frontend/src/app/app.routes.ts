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
import { LeaveBalanceComponent } from './features/employee/leave-balance/leave-balance.component';
import { CalendarComponent } from './features/employee/calendar/calendar.component';
import { EmployeeReportsComponent } from './features/employee/employee-reports/employee-reports.component';

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
import { BenefitsComponent } from './features/public/benefits/benefits.component';
import { FaqComponent } from './features/public/faq/faq.component';
import { ContactComponent } from './features/public/contact/contact.component';
import { NotFoundComponent } from './features/public/not-found/not-found.component';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'benefits', component: BenefitsComponent },
  { path: 'faq', component: FaqComponent },
  { path: 'contact', component: ContactComponent },
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
        data: { roles: ['Employee', 'Manager', 'Admin', 'HR'] }
      },
      {
        path: 'employee/apply',
        component: ApplyLeaveComponent,
        canActivate: [roleGuard],
        data: { roles: ['Employee', 'Manager', 'Admin', 'HR'] }
      },
      {
        path: 'employee/requests',
        component: MyRequestsComponent,
        canActivate: [roleGuard],
        data: { roles: ['Employee', 'Manager', 'Admin', 'HR'] }
      },
      {
        path: 'employee/attendance',
        component: MyAttendanceComponent,
        canActivate: [roleGuard],
        data: { roles: ['Employee', 'Manager', 'Admin', 'HR'] }
      },
      {
        path: 'employee/leave-balance',
        component: LeaveBalanceComponent,
        canActivate: [roleGuard],
        data: { roles: ['Employee', 'Manager', 'Admin', 'HR'] }
      },
      {
        path: 'employee/calendar',
        component: CalendarComponent,
        canActivate: [roleGuard],
        data: { roles: ['Employee', 'Manager', 'Admin', 'HR'] }
      },
      {
        path: 'employee/reports',
        component: EmployeeReportsComponent,
        canActivate: [roleGuard],
        data: { roles: ['Employee', 'Manager', 'Admin', 'HR'] }
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
        loadComponent: () => import('./features/admin/dashboard.component').then(m => m.AdminDashboardComponent),
        canActivate: [roleGuard], data: { roles: ['Admin', 'HR'] }
      },
      { 
        path: 'admin/employees', 
        loadComponent: () => import('./features/admin/employees.component').then(m => m.EmployeesComponent),
        canActivate: [roleGuard], data: { roles: ['Admin', 'HR'] }
      },
      { 
        path: 'admin/organization', 
        loadComponent: () => import('./features/admin/organization.component').then(m => m.OrganizationComponent),
        canActivate: [roleGuard], data: { roles: ['Admin', 'HR'] }
      },
      { 
        path: 'admin/holidays', 
        loadComponent: () => import('./features/admin/holidays.component').then(m => m.HolidaysComponent),
        canActivate: [roleGuard], data: { roles: ['Admin', 'HR'] }
      },
      {
        path: 'admin/leave-types',
        component: LeaveTypesComponent,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      },
      { 
        path: 'admin/reports', 
        loadComponent: () => import('./features/admin/reports.component').then(m => m.ReportsComponent),
        canActivate: [roleGuard], data: { roles: ['Admin', 'HR'] }
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

  { path: '**', component: NotFoundComponent }
];

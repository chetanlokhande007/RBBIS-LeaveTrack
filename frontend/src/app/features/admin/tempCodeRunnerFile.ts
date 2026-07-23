import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TitleService } from '../../core/services/title.service';
import { EmployeeService } from '../../core/services/employee.service';
import { LeaveService } from '../../core/services/leave.service';
import { AttendanceService } from '../../core/services/attendance.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html'
})
export class AdminDashboardComponent implements OnInit {
  totalEmployees = 0;
  leavesToday = 0;
  pendingApprovals = 0;
  holidaysCount = 0;

  deptBreakdown = [
    { name: 'Engineering', percentage: 84, val: '84%' },
    { name: 'Human Resources', percentage: 100, val: '100%' },
    { name: 'Sales', percentage: 0, val: '0%' }
  ];

  constructor(
    private titleService: TitleService,
    private employeeService: EmployeeService,
    private leaveService: LeaveService,
    private attendanceService: AttendanceService
  ) {}

  ngOnInit() {
    this.titleService.setTitle('Admin Dashboard');
    this.loadMetrics();
  }

  loadMetrics() {
    // 1. Get employee count
    this.employeeService.getAll().subscribe(employees => {
      this.totalEmployees = employees.length;
    });

    // 2. Count active leaves today
    this.leaveService.getLeaveRequests('approved').subscribe(requests => {
      const today = new Date().toISOString().split('T')[0];
      this.leavesToday = requests.filter(r => r.startDate <= today && r.endDate >= today).length;
    });

    // 3. Count pending approvals
    this.leaveService.getLeaveRequests('pending').subscribe(requests => {
      this.pendingApprovals = requests.length;
    });

    // 4. Count holidays
    this.leaveService.getHolidays().subscribe(holidays => {
      this.holidaysCount = holidays.length;
    });
  }
}

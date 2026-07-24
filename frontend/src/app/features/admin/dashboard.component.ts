import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TitleService } from '../../core/services/title.service';
import { EmployeeService } from '../../core/services/employee.service';
import { LeaveService } from '../../core/services/leave.service';
import { AttendanceService } from '../../core/services/attendance.service';
import { AuthService } from '../../core/services/auth.service';

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
  presentToday = 0;
  absentToday = 0;

  deptBreakdown = [
    { name: 'Engineering', percentage: 84, val: '84%' },
    { name: 'Human Resources', percentage: 100, val: '100%' },
    { name: 'Sales', percentage: 0, val: '0%' }
  ];

  recentRequests: any[] = [];
  rejectedLeaves = 0;
  holidays: any[] = [];
  currentDate = new Date();

  leaveStats = {
    annual: { count: 0, percent: 0 },
    casual: { count: 0, percent: 0 },
    sick: { count: 0, percent: 0 },
    other: { count: 0, percent: 0 },
    total: 0
  };

  balanceStats = {
    total: 0,
    annual: 0,
    casual: 0,
    sick: 0,
    other: 0
  };

  constructor(
    private titleService: TitleService,
    private employeeService: EmployeeService,
    private leaveService: LeaveService,
    private attendanceService: AttendanceService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    const role = this.authService.userRole;
    this.titleService.setTitle(role === 'HR' ? 'HR Dashboard' : 'Admin Dashboard');
    this.loadMetrics();
  }

  loadMetrics() {
    // 1. Get employee count
    this.employeeService.getAll().subscribe(employees => {
      this.totalEmployees = employees.length;
    });

    // 2. Count active leaves today & recent requests
    this.leaveService.getLeaveRequests('all').subscribe(requests => {
      const today = new Date().toISOString().split('T')[0];
      this.leavesToday = requests.filter(r => r.status === 'Approved' && r.startDate <= today && r.endDate >= today).length;
      this.pendingApprovals = requests.filter(r => r.status === 'Pending').length;
      this.rejectedLeaves = requests.filter(r => r.status === 'Rejected').length;
      
      // Calculate mock attendance (Phase 1)
      this.presentToday = Math.max(0, this.totalEmployees - this.leavesToday - 1);
      this.absentToday = this.totalEmployees > 0 ? 1 : 0; // Mock 1 absent employee for display

      // Sort by appliedOn (newest first) and take top 5
      this.recentRequests = requests.sort((a, b) => new Date(b.appliedOn).getTime() - new Date(a.appliedOn).getTime()).slice(0, 5);

      // Leave Stats
      const currentMonth = new Date().getMonth();
      const currentYear = new Date().getFullYear();
      const thisMonthRequests = requests.filter(r => {
        const d = new Date(r.startDate);
        return d.getMonth() === currentMonth && d.getFullYear() === currentYear;
      });
      
      this.leaveStats.total = thisMonthRequests.length;
      if (this.leaveStats.total > 0) {
        this.leaveStats.annual.count = thisMonthRequests.filter(r => r.leaveTypeName === 'Annual Leave').length;
        this.leaveStats.casual.count = thisMonthRequests.filter(r => r.leaveTypeName === 'Casual Leave').length;
        this.leaveStats.sick.count = thisMonthRequests.filter(r => r.leaveTypeName === 'Sick Leave').length;
        this.leaveStats.other.count = this.leaveStats.total - this.leaveStats.annual.count - this.leaveStats.casual.count - this.leaveStats.sick.count;
        
        this.leaveStats.annual.percent = Math.round((this.leaveStats.annual.count / this.leaveStats.total) * 100);
        this.leaveStats.casual.percent = Math.round((this.leaveStats.casual.count / this.leaveStats.total) * 100);
        this.leaveStats.sick.percent = Math.round((this.leaveStats.sick.count / this.leaveStats.total) * 100);
        this.leaveStats.other.percent = Math.round((this.leaveStats.other.count / this.leaveStats.total) * 100);
      }
    });

    // 3. Count holidays
    this.leaveService.getHolidays().subscribe(holidays => {
      this.holidays = holidays.sort((a: any, b: any) => new Date(a.date).getTime() - new Date(b.date).getTime());
      this.holidaysCount = this.holidays.length;
    });

    // 4. Get overall leave balances
    this.leaveService.getLeaveBalances().subscribe(balances => {
       let total = 0, annual = 0, casual = 0, sick = 0, other = 0;
       balances.forEach((emp: any) => {
           emp.leaveSummaries.forEach((sum: any) => {
               total += sum.remainingDays;
               if (sum.leaveTypeName === 'Annual Leave') annual += sum.remainingDays;
               else if (sum.leaveTypeName === 'Casual Leave') casual += sum.remainingDays;
               else if (sum.leaveTypeName === 'Sick Leave') sick += sum.remainingDays;
               else other += sum.remainingDays;
           });
       });
       this.balanceStats = { total, annual, casual, sick, other };
    });
  }
}

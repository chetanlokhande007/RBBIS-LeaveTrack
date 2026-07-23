import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TitleService } from '../../core/services/title.service';
import { LeaveService } from '../../core/services/leave.service';
import { EmployeeService } from '../../core/services/employee.service';
import { AttendanceService } from '../../core/services/attendance.service';
import { forkJoin, map } from 'rxjs';

@Component({
  selector: 'app-admin-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reports.component.html'
})
export class ReportsComponent implements OnInit {
  reportType: 'leave' | 'attendance' = 'leave';
  startDate = '';
  endDate = '';
  department = '';
  
  leaveReportData: any[] = [];
  filteredLeaveData: any[] = [];
  
  attendanceReportData: any[] = [];
  filteredAttendanceData: any[] = [];

  constructor(
    private titleService: TitleService,
    private leaveService: LeaveService,
    private employeeService: EmployeeService,
    private attendanceService: AttendanceService
  ) {
    // Set default dates for last 30 days
    const today = new Date();
    const lastMonth = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000);
    this.endDate = today.toISOString().split('T')[0];
    this.startDate = lastMonth.toISOString().split('T')[0];
  }

  ngOnInit() {
    this.titleService.setTitle('Reports & Analytics');
    this.loadLeaveReport();
    this.loadAttendanceReport();
  }

  loadLeaveReport() {
    this.leaveService.getLeaveBalances().subscribe({
      next: (res) => {
        this.leaveReportData = res;
        this.applyFilters();
      }
    });
  }

  loadAttendanceReport() {
    this.employeeService.getAll().subscribe(employees => {
      // For each employee, load attendance summary in parallel
      const obs = employees.map(emp => 
        this.attendanceService.getAttendanceSummary(this.startDate, this.endDate, emp.id).pipe(
          map(summary => ({
            employeeId: emp.id,
            fullName: emp.fullName,
            department: emp.department,
            designation: emp.designation,
            ...summary
          }))
        )
      );

      forkJoin(obs).subscribe({
        next: (results) => {
          this.attendanceReportData = results;
          this.applyFilters();
        }
      });
    });
  }

  applyFilters() {
    const dept = this.department.trim().toLowerCase();

    // 1. Filter Leave Balances
    if (dept) {
      // Find employee department by matching database employees
      this.employeeService.getAll().subscribe(employees => {
        const empDepts = new Map(employees.map(e => [e.id, e.department.toLowerCase()]));
        this.filteredLeaveData = this.leaveReportData.filter(item => 
          empDepts.get(item.employeeId) === dept
        );
      });
    } else {
      this.filteredLeaveData = this.leaveReportData;
    }

    // 2. Filter Attendance summaries
    if (dept) {
      this.filteredAttendanceData = this.attendanceReportData.filter(item => 
        item.department.toLowerCase() === dept
      );
    } else {
      this.filteredAttendanceData = this.attendanceReportData;
    }
  }

  onFilterSubmit() {
    this.loadAttendanceReport();
    this.applyFilters();
  }
}

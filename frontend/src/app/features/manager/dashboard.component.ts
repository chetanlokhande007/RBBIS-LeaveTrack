import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TitleService } from '../../core/services/title.service';
import { AttendanceService } from '../../core/services/attendance.service';
import { LeaveService } from '../../core/services/leave.service';
import { EmployeeService } from '../../core/services/employee.service';

@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html'
})
export class ManagerDashboardComponent implements OnInit {
  teamSize = 0;
  presentToday = 0;
  onLeaveToday = 0;
  pendingApprovals = 0;
  
  teamList: any[] = [];
  recentTeamAttendance: any[] = [];

  constructor(
    private titleService: TitleService,
    private attendanceService: AttendanceService,
    private leaveService: LeaveService,
    private employeeService: EmployeeService
  ) {}

  ngOnInit() {
    this.titleService.setTitle('Manager Dashboard');
    this.loadTeamMetrics();
  }

  loadTeamMetrics() {
    // 1. Get team size (direct reports)
    this.employeeService.getAll().subscribe(employees => {
      this.teamList = employees;
      this.teamSize = employees.length;
      
      // 2. Get today's team attendance
      const todayStr = new Date().toISOString().split('T')[0];
      this.attendanceService.getTeamAttendance(todayStr).subscribe(attendance => {
        this.recentTeamAttendance = attendance;
        this.presentToday = attendance.filter(a => a.status === 0 || a.status === 2).length; // Present or HalfDay
        this.onLeaveToday = attendance.filter(a => a.status === 3).length; // Leave
        
        // Map attendance to team list for display
        this.teamList = this.teamList.map(emp => {
          const record = attendance.find(a => a.employeeId === emp.id);
          let status: 'present' | 'absent' | 'leave' = 'absent';
          if (record) {
            if (record.status === 0 || record.status === 2) status = 'present';
            else if (record.status === 3) status = 'leave';
          }
          return {
            ...emp,
            status,
            initials: emp.fullName.split(' ').map((n: string) => n[0]).join('').substring(0, 2).toUpperCase()
          };
        });
      });
    });

    // 3. Count pending approvals
    this.leaveService.getLeaveRequests('pending').subscribe(requests => {
      this.pendingApprovals = requests.length;
    });
  }
}

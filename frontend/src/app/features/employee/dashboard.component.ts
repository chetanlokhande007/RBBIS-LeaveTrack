import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TitleService } from '../../core/services/title.service';
import { AttendanceService } from '../../core/services/attendance.service';
import { LeaveService } from '../../core/services/leave.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-employee-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html'
})
export class EmployeeDashboardComponent implements OnInit, OnDestroy {
  clockTime = '--:--';
  private clockInterval: any;
  
  checkedIn = false;
  statusText = 'Loading punch status...';
  checkInTime: string | null = null;
  checkOutTime: string | null = null;
  
  leaveBalances: any[] = [];
  attendanceSummary: any = null;
  recentRequests: any[] = [];

  constructor(
    private titleService: TitleService,
    private attendanceService: AttendanceService,
    private leaveService: LeaveService,
    private notification: NotificationService
  ) {}

  ngOnInit() {
    this.titleService.setTitle('Employee Dashboard');
    this.updateClock();
    this.clockInterval = setInterval(() => this.updateClock(), 1000);
    
    this.loadTodayStatus();
    this.loadLeaveBalances();
    this.loadAttendanceSummary();
    this.loadRecentRequests();
  }

  ngOnDestroy() {
    if (this.clockInterval) {
      clearInterval(this.clockInterval);
    }
  }

  updateClock() {
    const now = new Date();
    this.clockTime = now.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false
    });
  }

  loadTodayStatus() {
    this.attendanceService.getTodayStatus().subscribe({
      next: (res) => {
        this.checkedIn = res.checkedIn;
        if (res.checkOutTime) {
          const time = new Date(res.checkOutTime).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
          this.statusText = `Checked out at ${time}`;
        } else if (res.checkInTime) {
          const time = new Date(res.checkInTime).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
          this.statusText = `Checked in at ${time}`;
        } else {
          this.statusText = 'Not checked in yet today';
        }
      }
    });
  }

  loadLeaveBalances() {
    this.leaveService.getLeaveBalances().subscribe({
      next: (res) => {
        if (res && res.length > 0) {
          this.leaveBalances = res[0].leaveSummaries;
        }
      }
    });
  }

  loadAttendanceSummary() {
    this.attendanceService.getAttendanceSummary().subscribe({
      next: (res) => {
        this.attendanceSummary = res;
      }
    });
  }

  loadRecentRequests() {
    this.leaveService.getLeaveRequests().subscribe({
      next: (res) => {
        this.recentRequests = res.slice(0, 5); // get top 5 requests
      }
    });
  }

  toggleCheckIn() {
    if (!this.checkedIn) {
      this.attendanceService.checkIn().subscribe({
        next: (res) => {
          this.checkedIn = true;
          const time = new Date(res.checkInTime).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
          this.statusText = `Checked in at ${time}`;
          this.notification.show(`Checked in successfully at ${time}`, 'success');
          this.loadAttendanceSummary();
        },
        error: () => this.notification.show('Failed to check in', 'error')
      });
    } else {
      this.attendanceService.checkOut().subscribe({
        next: (res) => {
          this.checkedIn = false;
          const time = new Date(res.checkOutTime).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
          this.statusText = `Checked out at ${time}`;
          this.notification.show(`Checked out successfully at ${time}`, 'success');
          this.loadAttendanceSummary();
          this.loadTodayStatus();
        },
        error: () => this.notification.show('Failed to check out', 'error')
      });
    }
  }

  getStatusBadgeClass(status: number): string {
    // leave request status enum: Pending=0, Approved=1, Rejected=2
    switch (status) {
      case 0: return 'badge badge-pending';
      case 1: return 'badge badge-approved';
      case 2: return 'badge badge-rejected';
      default: return 'badge';
    }
  }

  getStatusText(status: number): string {
    switch (status) {
      case 0: return 'Pending';
      case 1: return 'Approved';
      case 2: return 'Rejected';
      default: return '';
    }
  }
}

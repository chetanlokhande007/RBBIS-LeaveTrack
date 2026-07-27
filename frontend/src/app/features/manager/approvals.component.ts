import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TitleService } from '../../core/services/title.service';
import { LeaveService } from '../../core/services/leave.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-manager-approvals',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './approvals.component.html'
})
export class ApprovalsComponent implements OnInit {
  pendingRequests: any[] = [];
  loading = false;
  remarksMap: { [key: number]: string } = {};

  constructor(
    private titleService: TitleService,
    private leaveService: LeaveService,
    private notification: NotificationService
  ) {}

  ngOnInit() {
    this.titleService.setTitle('Approvals Inbox');
    this.loadPendingRequests();
  }

  loadPendingRequests() {
    this.loading = true;
    this.leaveService.getLeaveRequests('pending').subscribe({
      next: (res) => {
        this.pendingRequests = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.show('Failed to load pending requests', 'error');
      }
    });
  }

  decideRequest(id: number, approved: boolean) {
    const statusVal = approved ? 1 : 2; // Approved=1, Rejected=2
    const remarks = this.remarksMap[id] || '';

    if (!approved && !remarks.trim()) {
      this.notification.show('Remarks are mandatory when rejecting.', 'error');
      return;
    }

    this.leaveService.decideLeaveRequest(id, statusVal, remarks).subscribe({
      next: () => {
        const actionStr = approved ? 'approved' : 'rejected';
        this.notification.show(`Leave request successfully ${actionStr}!`, 'success');
        delete this.remarksMap[id];
        this.loadPendingRequests();
      },
      error: (err) => {
        const msg = err.error?.message || 'Action failed';
        this.notification.show(msg, 'error');
      }
    });
  }

  calculateDays(startDate: string, endDate: string): number {
    const start = new Date(startDate);
    const end = new Date(endDate);
    return Math.floor((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)) + 1;
  }
}

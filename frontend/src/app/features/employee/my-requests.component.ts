import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TitleService } from '../../core/services/title.service';
import { LeaveService } from '../../core/services/leave.service';
import { NotificationService } from '../../core/services/notification.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-my-requests',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './my-requests.component.html'
})
export class MyRequestsComponent implements OnInit {
  requests: any[] = [];
  filteredRequests: any[] = [];
  
  currentFilter: 'all' | 'pending' | 'approved' | 'rejected' = 'all';
  searchQuery = '';

  constructor(
    private titleService: TitleService,
    private leaveService: LeaveService,
    private notification: NotificationService
  ) {}

  ngOnInit() {
    this.titleService.setTitle('My Requests');
    this.loadRequests();
  }

  loadRequests() {
    this.leaveService.getLeaveRequests().subscribe({
      next: (res) => {
        this.requests = res;
        this.applyFilters();
      }
    });
  }

  setFilter(filter: 'all' | 'pending' | 'approved' | 'rejected') {
    this.currentFilter = filter;
    this.applyFilters();
  }

  onSearchChange() {
    this.applyFilters();
  }

  applyFilters() {
    let result = this.requests;

    // Apply status filter
    if (this.currentFilter !== 'all') {
      const statusMap = { pending: 0, approved: 1, rejected: 2 };
      const statusVal = statusMap[this.currentFilter];
      result = result.filter(r => r.status === statusVal);
    }

    // Apply search filter
    if (this.searchQuery) {
      const q = this.searchQuery.toLowerCase();
      result = result.filter(r => 
        r.leaveTypeName.toLowerCase().includes(q) || 
        r.reason.toLowerCase().includes(q) ||
        (r.approvedByName && r.approvedByName.toLowerCase().includes(q))
      );
    }

    this.filteredRequests = result;
  }

  cancelRequest(id: number) {
    if (confirm('Are you sure you want to cancel this leave request?')) {
      this.leaveService.cancelLeaveRequest(id).subscribe({
        next: () => {
          this.notification.show('Leave request cancelled successfully', 'success');
          this.loadRequests();
        },
        error: () => this.notification.show('Failed to cancel request', 'error')
      });
    }
  }

  getStatusBadgeClass(status: number): string {
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

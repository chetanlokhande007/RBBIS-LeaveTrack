import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TitleService } from '../../core/services/title.service';

@Component({
  selector: 'app-admin-audit-log',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './audit-log.component.html'
})
export class AuditLogComponent implements OnInit {
  logs = [
    { timestamp: '2026-07-05 10:15:32', user: 'admin', action: 'User Login', details: 'Admin logged in from IP 127.0.0.1' },
    { timestamp: '2026-07-05 09:30:14', user: 'manager1', action: 'Leave Decision', details: 'Approved Casual Leave for Alice Cooper (ID: 3)' },
    { timestamp: '2026-07-04 15:45:00', user: 'admin', action: 'Employee Created', details: 'Registered Bob Johnson (ID: 4) into HR Database' },
    { timestamp: '2026-07-04 09:12:05', user: 'employee1', action: 'Check In', details: 'Logged check-in punch at 09:12 AM' },
    { timestamp: '2026-07-03 18:30:22', user: 'employee2', action: 'Check Out', details: 'Logged check-out punch at 06:30 PM' },
    { timestamp: '2026-07-03 14:10:00', user: 'admin', action: 'Holiday Created', details: 'Registered Independence Day holiday on Aug 15' }
  ];

  constructor(private titleService: TitleService) {}

  ngOnInit() {
    this.titleService.setTitle('Audit Trail');
  }
}

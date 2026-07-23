import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TitleService } from '../../core/services/title.service';
import { LeaveService } from '../../core/services/leave.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-apply-leave',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './apply-leave.component.html'
})
export class ApplyLeaveComponent implements OnInit {
  leaveForm: FormGroup;
  leaveTypes: any[] = [];
  leaveBalances: any[] = [];
  selectedBalanceHint = '—';
  submitting = false;

  constructor(
    private fb: FormBuilder,
    private titleService: TitleService,
    private leaveService: LeaveService,
    private notification: NotificationService,
    private router: Router
  ) {
    this.leaveForm = this.fb.group({
      leaveTypeId: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      reason: ['', [Validators.required, Validators.maxLength(500)]]
    });
  }

  ngOnInit() {
    this.titleService.setTitle('Apply Leave');
    this.loadLeaveTypesAndBalances();

    // Watch for leave type changes to update balance hint
    this.leaveForm.get('leaveTypeId')?.valueChanges.subscribe(val => {
      this.updateBalanceHint(val);
    });
  }

  loadLeaveTypesAndBalances() {
    // Load leave types
    this.leaveService.getLeaveTypes().subscribe(types => {
      this.leaveTypes = types;
      
      // Load balances to match hints
      this.leaveService.getLeaveBalances().subscribe(balances => {
        if (balances && balances.length > 0) {
          this.leaveBalances = balances[0].leaveSummaries;
          // Trigger balance hint update for default selected (if any)
          const firstVal = this.leaveForm.get('leaveTypeId')?.value;
          if (firstVal) this.updateBalanceHint(firstVal);
        }
      });
    });
  }

  updateBalanceHint(leaveTypeId: any) {
    if (!leaveTypeId) {
      this.selectedBalanceHint = '—';
      return;
    }
    const balanceObj = this.leaveBalances.find(b => b.leaveTypeId === Number(leaveTypeId));
    if (balanceObj) {
      this.selectedBalanceHint = `${balanceObj.remainingDays} days`;
    } else {
      this.selectedBalanceHint = '—';
    }
  }

  onSubmit() {
    if (this.leaveForm.invalid) return;

    const formVal = this.leaveForm.value;
    
    // Validate end date is not before start date
    if (new Date(formVal.startDate) > new Date(formVal.endDate)) {
      this.notification.show('End date cannot be before start date', 'error');
      return;
    }

    this.submitting = true;
    
    const requestPayload = {
      leaveTypeId: Number(formVal.leaveTypeId),
      startDate: formVal.startDate,
      endDate: formVal.endDate,
      reason: formVal.reason
    };

    this.leaveService.applyLeave(requestPayload).subscribe({
      next: () => {
        this.submitting = false;
        this.notification.show('Leave request submitted successfully!', 'success');
        this.router.navigate(['/employee/requests']);
      },
      error: (err) => {
        this.submitting = false;
        const msg = err.error || 'Failed to submit leave request';
        this.notification.show(msg, 'error');
      }
    });
  }
}

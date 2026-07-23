import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TitleService } from '../../core/services/title.service';
import { LeaveService } from '../../core/services/leave.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-admin-leave-types',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './leave-types.component.html'
})
export class LeaveTypesComponent implements OnInit {
  activeTab: 'types' | 'holidays' = 'types';
  
  leaveTypes: any[] = [];
  holidays: any[] = [];
  
  // Modals status
  showTypeModal = false;
  isTypeEdit = false;
  selectedTypeId: number | null = null;
  typeForm: FormGroup;
  submittingType = false;

  showHolidayModal = false;
  isHolidayEdit = false;
  selectedHolidayId: number | null = null;
  holidayForm: FormGroup;
  submittingHoliday = false;

  constructor(
    private fb: FormBuilder,
    private titleService: TitleService,
    private leaveService: LeaveService,
    private notification: NotificationService
  ) {
    this.typeForm = this.fb.group({
      name: ['', Validators.required],
      defaultDaysPerYear: [10, [Validators.required, Validators.min(1), Validators.max(365)]]
    });

    this.holidayForm = this.fb.group({
      name: ['', Validators.required],
      date: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.titleService.setTitle('Configuration Rules');
    this.loadLeaveTypes();
    this.loadHolidays();
  }

  setTab(tab: 'types' | 'holidays') {
    this.activeTab = tab;
  }

  loadLeaveTypes() {
    this.leaveService.getLeaveTypes().subscribe({
      next: (res) => this.leaveTypes = res
    });
  }

  loadHolidays() {
    this.leaveService.getHolidays().subscribe({
      next: (res) => this.holidays = res
    });
  }

  // --- Leave Type Actions ---
  openAddType() {
    this.isTypeEdit = false;
    this.selectedTypeId = null;
    this.typeForm.reset({ defaultDaysPerYear: 10 });
    this.showTypeModal = true;
  }

  openEditType(type: any) {
    this.isTypeEdit = true;
    this.selectedTypeId = type.id;
    this.typeForm.patchValue({
      name: type.name,
      defaultDaysPerYear: type.defaultDaysPerYear
    });
    this.showTypeModal = true;
  }

  saveType() {
    if (this.typeForm.invalid) return;
    this.submittingType = true;
    const val = this.typeForm.value;

    if (this.isTypeEdit && this.selectedTypeId) {
      this.leaveService.updateLeaveType(this.selectedTypeId, { id: this.selectedTypeId, ...val }).subscribe({
        next: () => {
          this.submittingType = false;
          this.showTypeModal = false;
          this.notification.show('Leave type updated successfully', 'success');
          this.loadLeaveTypes();
        },
        error: () => {
          this.submittingType = false;
          this.notification.show('Failed to update leave type', 'error');
        }
      });
    } else {
      this.leaveService.createLeaveType(val).subscribe({
        next: () => {
          this.submittingType = false;
          this.showTypeModal = false;
          this.notification.show('New leave type created successfully', 'success');
          this.loadLeaveTypes();
        },
        error: () => {
          this.submittingType = false;
          this.notification.show('Failed to create leave type', 'error');
        }
      });
    }
  }

  deleteType(id: number) {
    if (confirm('Delete this leave category? All associated request history will be removed.')) {
      this.leaveService.deleteLeaveType(id).subscribe({
        next: () => {
          this.notification.show('Leave category deleted', 'success');
          this.loadLeaveTypes();
        },
        error: () => this.notification.show('Failed to delete leave category', 'error')
      });
    }
  }

  // --- Holiday Actions ---
  openAddHoliday() {
    this.isHolidayEdit = false;
    this.selectedHolidayId = null;
    this.holidayForm.reset();
    this.showHolidayModal = true;
  }

  openEditHoliday(holiday: any) {
    this.isHolidayEdit = true;
    this.selectedHolidayId = holiday.id;
    this.holidayForm.patchValue({
      name: holiday.name,
      date: holiday.date
    });
    this.showHolidayModal = true;
  }

  saveHoliday() {
    if (this.holidayForm.invalid) return;
    this.submittingHoliday = true;
    const val = this.holidayForm.value;

    if (this.isHolidayEdit && this.selectedHolidayId) {
      this.leaveService.updateHoliday(this.selectedHolidayId, { id: this.selectedHolidayId, ...val }).subscribe({
        next: () => {
          this.submittingHoliday = false;
          this.showHolidayModal = false;
          this.notification.show('Holiday updated successfully', 'success');
          this.loadHolidays();
        },
        error: () => {
          this.submittingHoliday = false;
          this.showHolidayModal = false;
          this.notification.show('Failed to update holiday', 'error');
        }
      });
    } else {
      this.leaveService.createHoliday(val).subscribe({
        next: () => {
          this.submittingHoliday = false;
          this.showHolidayModal = false;
          this.notification.show('New holiday registered successfully', 'success');
          this.loadHolidays();
        },
        error: () => {
          this.submittingHoliday = false;
          this.notification.show('Failed to register holiday', 'error');
        }
      });
    }
  }

  deleteHoliday(id: number) {
    if (confirm('Are you sure you want to delete this holiday?')) {
      this.leaveService.deleteHoliday(id).subscribe({
        next: () => {
          this.notification.show('Holiday removed', 'success');
          this.loadHolidays();
        },
        error: () => this.notification.show('Failed to remove holiday', 'error')
      });
    }
  }
}

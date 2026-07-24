import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TitleService } from '../../core/services/title.service';
import { HolidayService } from '../../core/services/holiday.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-admin-holidays',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './holidays.component.html',
  styleUrls: ['./holidays.component.css']
})
export class HolidaysComponent implements OnInit {
  holidays: any[] = [];
  
  currentDate = new Date();
  currentMonth: number = this.currentDate.getMonth();
  currentYear: number = this.currentDate.getFullYear();
  
  calendarDays: any[] = [];
  months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
  weekdays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

  showModal = false;
  isEdit = false;
  selectedId: number | null = null;
  form: FormGroup;
  submitting = false;

  constructor(
    private fb: FormBuilder,
    private titleService: TitleService,
    private holidayService: HolidayService,
    private notification: NotificationService
  ) {
    this.form = this.fb.group({
      date: ['', Validators.required],
      name: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit() {
    this.titleService.setTitle('Holiday Calendar');
    this.loadData();
  }

  loadData() {
    this.holidayService.getAll().subscribe({
      next: (res) => {
        this.holidays = res;
        this.generateCalendar();
      }
    });
  }

  generateCalendar() {
    this.calendarDays = [];
    const firstDay = new Date(this.currentYear, this.currentMonth, 1).getDay();
    const daysInMonth = new Date(this.currentYear, this.currentMonth + 1, 0).getDate();

    // Previous month filler days
    for (let i = 0; i < firstDay; i++) {
      this.calendarDays.push({ date: null, isCurrentMonth: false });
    }

    // Current month days
    for (let i = 1; i <= daysInMonth; i++) {
      const dateObj = new Date(this.currentYear, this.currentMonth, i);
      const dateString = dateObj.toISOString().split('T')[0];
      
      const dayHolidays = this.holidays.filter(h => h.date.startsWith(dateString));
      
      this.calendarDays.push({
        date: i,
        dateString: dateString,
        isCurrentMonth: true,
        holidays: dayHolidays
      });
    }
  }

  prevMonth() {
    if (this.currentMonth === 0) {
      this.currentMonth = 11;
      this.currentYear--;
    } else {
      this.currentMonth--;
    }
    this.generateCalendar();
  }

  nextMonth() {
    if (this.currentMonth === 11) {
      this.currentMonth = 0;
      this.currentYear++;
    } else {
      this.currentMonth++;
    }
    this.generateCalendar();
  }

  openAddModal(dateString?: string) {
    this.isEdit = false;
    this.selectedId = null;
    this.form.reset();
    if (dateString) {
      this.form.patchValue({ date: dateString });
    }
    this.showModal = true;
  }

  openEditModal(item: any, event: Event) {
    event.stopPropagation();
    this.isEdit = true;
    this.selectedId = item.id;
    this.form.patchValue({
      date: item.date.split('T')[0],
      name: item.name,
      description: item.description
    });
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  save() {
    if (this.form.invalid) return;
    this.submitting = true;
    const val = this.form.value;

    if (this.isEdit && this.selectedId) {
      this.holidayService.update(this.selectedId, val).subscribe({
        next: () => {
          this.submitting = false;
          this.showModal = false;
          this.notification.show('Holiday updated successfully', 'success');
          this.loadData();
        },
        error: () => {
          this.submitting = false;
          this.notification.show('Failed to update holiday', 'error');
        }
      });
    } else {
      this.holidayService.create(val).subscribe({
        next: () => {
          this.submitting = false;
          this.showModal = false;
          this.notification.show('Holiday created successfully', 'success');
          this.loadData();
        },
        error: () => {
          this.submitting = false;
          this.notification.show('Failed to create holiday', 'error');
        }
      });
    }
  }

  deleteItem(id: number, event: Event) {
    event.stopPropagation();
    if (confirm('Are you sure you want to delete this holiday?')) {
      this.holidayService.delete(id).subscribe({
        next: () => {
          this.notification.show('Holiday deleted', 'success');
          this.loadData();
        },
        error: () => this.notification.show('Failed to delete holiday', 'error')
      });
    }
  }
}

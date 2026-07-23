import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TitleService } from '../../core/services/title.service';
import { AttendanceService } from '../../core/services/attendance.service';

interface CalendarCell {
  dayNumber?: number;
  isEmpty: boolean;
  status?: 'present' | 'absent' | 'leave' | 'weekend' | 'today';
  badgeText?: string;
  badgeClass?: string;
  timeRange?: string;
  isToday?: boolean;
}

@Component({
  selector: 'app-my-attendance',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-attendance.component.html'
})
export class MyAttendanceComponent implements OnInit {
  currentMonthName = 'July 2026';
  daysOfWeek = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
  calendarCells: CalendarCell[] = [];
  
  // July 2026 configuration (matches prototype)
  private readonly year = 2026;
  private readonly month = 7;
  private readonly firstDayOffset = 3; // July 2026 starts on Wednesday
  private readonly daysInMonth = 31;

  constructor(
    private titleService: TitleService,
    private attendanceService: AttendanceService
  ) {}

  ngOnInit() {
    this.titleService.setTitle('My Attendance');
    this.generateCalendar();
  }

  generateCalendar() {
    this.attendanceService.getMyHistory(this.month, this.year).subscribe({
      next: (dbRecords) => {
        const cells: CalendarCell[] = [];

        // Add empty offset cells
        for (let i = 0; i < this.firstDayOffset; i++) {
          cells.push({ isEmpty: true });
        }

        // Mock status map from prototype
        const mockStatusMap: { [key: number]: 'weekend' | 'leave' | 'absent' | 'present' } = {
          5: 'weekend', 6: 'weekend', 12: 'weekend', 13: 'weekend', 
          19: 'weekend', 20: 'weekend', 26: 'weekend', 27: 'weekend',
          14: 'leave', 15: 'leave', 16: 'leave', 
          9: 'absent'
        };

        const todayDate = new Date();
        const isCurrentMonth = todayDate.getFullYear() === this.year && (todayDate.getMonth() + 1) === this.month;
        const todayDayNum = todayDate.getDate();

        // Render days
        for (let d = 1; d <= this.daysInMonth; d++) {
          // Check if there is a DB record for this date
          const dateStr = `${this.year}-${this.month.toString().padStart(2, '0')}-${d.toString().padStart(2, '0')}`;
          const dbRecord = dbRecords.find(r => r.date === dateStr);

          let status: 'present' | 'absent' | 'leave' | 'weekend' = 'present';
          let timeRange = `09:1${d % 9} – 18:4${d % 5}`;
          
          if (dbRecord) {
            // Mapping API status enum: Present=0, Absent=1, HalfDay=2, Leave=3
            if (dbRecord.status === 0) status = 'present';
            else if (dbRecord.status === 1) status = 'absent';
            else if (dbRecord.status === 2) status = 'present'; // HalfDay
            else if (dbRecord.status === 3) status = 'leave';
            
            // Format check-in/out times
            if (dbRecord.checkInTime) {
              const inTime = new Date(dbRecord.checkInTime).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false });
              const outTime = dbRecord.checkOutTime 
                ? new Date(dbRecord.checkOutTime).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false })
                : '--:--';
              timeRange = `${inTime} – ${outTime}`;
            } else {
              timeRange = '';
            }
          } else {
            // Fallback to prototype mock data
            status = mockStatusMap[d] || 'present';
            if (status === 'weekend') {
              timeRange = '';
            } else if (status === 'leave') {
              timeRange = '';
            } else if (status === 'absent') {
              timeRange = '';
            }
          }

          const isToday = isCurrentMonth && d === todayDayNum;

          // Set badges details
          let badgeText = 'Present';
          let badgeClass = 'badge badge-present';

          if (status === 'weekend') {
            badgeText = 'Weekend';
            badgeClass = 'badge badge-weekend';
          } else if (status === 'leave') {
            badgeText = 'Leave';
            badgeClass = 'badge badge-leave';
          } else if (status === 'absent') {
            badgeText = 'Absent';
            badgeClass = 'badge badge-absent';
          }

          cells.push({
            dayNumber: d,
            isEmpty: false,
            status,
            badgeText,
            badgeClass,
            timeRange: timeRange || undefined,
            isToday
          });
        }

        this.calendarCells = cells;
      }
    });
  }
}

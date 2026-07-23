import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {
  constructor(private api: ApiService) {}

  getTodayStatus(): Observable<any> {
    return this.api.get<any>('attendance/today');
  }

  checkIn(): Observable<any> {
    return this.api.post<any>('attendance/checkin');
  }

  checkOut(): Observable<any> {
    return this.api.post<any>('attendance/checkout');
  }

  getMyHistory(month?: number, year?: number): Observable<any[]> {
    const params: any = {};
    if (month && year) {
      params.month = month;
      params.year = year;
    }
    return this.api.get<any[]>('attendance/my-history', params);
  }

  getTeamAttendance(date?: string, department?: string): Observable<any[]> {
    const params: any = {};
    if (date) params.date = date;
    if (department) params.department = department;
    return this.api.get<any[]>('attendance/team', params);
  }

  getAttendanceSummary(startDate?: string, endDate?: string, employeeId?: number): Observable<any> {
    const params: any = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    if (employeeId) params.targetEmployeeId = employeeId;
    return this.api.get<any>('reports/attendance-summary', params);
  }
}

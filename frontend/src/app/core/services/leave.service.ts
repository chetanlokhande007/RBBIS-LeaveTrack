import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LeaveService {
  constructor(private api: ApiService) {}

  getLeaveTypes(): Observable<any[]> {
    return this.api.get<any[]>('leavetypes');
  }

  createLeaveType(type: any): Observable<any> {
    return this.api.post<any>('leavetypes', type);
  }

  updateLeaveType(id: number, type: any): Observable<any> {
    return this.api.put<any>(`leavetypes/${id}`, type);
  }

  deleteLeaveType(id: number): Observable<any> {
    return this.api.delete<any>(`leavetypes/${id}`);
  }

  getLeaveRequests(status?: string): Observable<any[]> {
    const params = status ? { status } : {};
    return this.api.get<any[]>('leaverequests', params);
  }

  getLeaveRequestById(id: number): Observable<any> {
    return this.api.get<any>(`leaverequests/${id}`);
  }

  applyLeave(request: any): Observable<any> {
    return this.api.post<any>('leaverequests', request);
  }

  decideLeaveRequest(id: number, status: number): Observable<any> {
    return this.api.put<any>(`leaverequests/${id}/decide`, { status });
  }

  cancelLeaveRequest(id: number): Observable<any> {
    return this.api.delete<any>(`leaverequests/${id}`);
  }

  getHolidays(): Observable<any[]> {
    return this.api.get<any[]>('holidays');
  }

  createHoliday(holiday: any): Observable<any> {
    return this.api.post<any>('holidays', holiday);
  }

  updateHoliday(id: number, holiday: any): Observable<any> {
    return this.api.put<any>(`holidays/${id}`, holiday);
  }

  deleteHoliday(id: number): Observable<any> {
    return this.api.delete<any>(`holidays/${id}`);
  }

  getLeaveBalances(): Observable<any[]> {
    return this.api.get<any[]>('reports/leave-balance');
  }
}

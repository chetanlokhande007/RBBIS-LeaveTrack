import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  constructor(private api: ApiService) {}

  getAll(): Observable<any[]> {
    return this.api.get<any[]>('employees');
  }

  getPaged(page: number, pageSize: number, search?: string, department?: string, designation?: string): Observable<any> {
    let url = `employees/paged?page=${page}&pageSize=${pageSize}`;
    if (search) url += `&search=${encodeURIComponent(search)}`;
    if (department) url += `&department=${encodeURIComponent(department)}`;
    if (designation) url += `&designation=${encodeURIComponent(designation)}`;
    return this.api.get<any>(url);
  }

  getById(id: number): Observable<any> {
    return this.api.get<any>(`employees/${id}`);
  }

  create(employee: any): Observable<any> {
    return this.api.post<any>('employees', employee);
  }

  update(id: number, employee: any): Observable<any> {
    return this.api.put<any>(`employees/${id}`, employee);
  }

  delete(id: number): Observable<any> {
    return this.api.delete<any>(`employees/${id}`);
  }

  registerUser(userRegistration: any): Observable<any> {
    return this.api.post<any>('auth/register', userRegistration);
  }
}

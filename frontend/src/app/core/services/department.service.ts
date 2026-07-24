import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  constructor(private api: ApiService) {}

  getAll(): Observable<any[]> {
    return this.api.get<any[]>('departments');
  }

  getById(id: number): Observable<any> {
    return this.api.get<any>(`departments/${id}`);
  }

  create(data: any): Observable<any> {
    return this.api.post<any>('departments', data);
  }

  update(id: number, data: any): Observable<any> {
    return this.api.put<any>(`departments/${id}`, data);
  }

  delete(id: number): Observable<any> {
    return this.api.delete<any>(`departments/${id}`);
  }
}

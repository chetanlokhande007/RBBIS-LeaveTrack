import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DesignationService {
  constructor(private api: ApiService) {}

  getAll(): Observable<any[]> {
    return this.api.get<any[]>('designations');
  }

  getById(id: number): Observable<any> {
    return this.api.get<any>(`designations/${id}`);
  }

  create(data: any): Observable<any> {
    return this.api.post<any>('designations', data);
  }

  update(id: number, data: any): Observable<any> {
    return this.api.put<any>(`designations/${id}`, data);
  }

  delete(id: number): Observable<any> {
    return this.api.delete<any>(`designations/${id}`);
  }
}

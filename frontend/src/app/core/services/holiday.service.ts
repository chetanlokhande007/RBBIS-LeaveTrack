import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HolidayService {
  constructor(private api: ApiService) {}

  getAll(): Observable<any[]> {
    return this.api.get<any[]>('holidays');
  }

  getById(id: number): Observable<any> {
    return this.api.get<any>(`holidays/${id}`);
  }

  create(data: any): Observable<any> {
    return this.api.post<any>('holidays', data);
  }

  update(id: number, data: any): Observable<any> {
    return this.api.put<any>(`holidays/${id}`, data);
  }

  delete(id: number): Observable<any> {
    return this.api.delete<any>(`holidays/${id}`);
  }
}

import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { ApiService } from './api.service';
import { Router } from '@angular/router';

export interface UserSession {
  token: string;
  username: string;
  role: string;
  employeeId?: number;
  fullName: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<UserSession | null>;
  public currentUser$: Observable<UserSession | null>;

  constructor(private api: ApiService, private router: Router) {
    const storedSession = localStorage.getItem('leave_track_session');
    this.currentUserSubject = new BehaviorSubject<UserSession | null>(
      storedSession ? JSON.parse(storedSession) : null
    );
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): UserSession | null {
    return this.currentUserSubject.value;
  }

  public get isLoggedIn(): boolean {
    return this.currentUserValue !== null;
  }

  public get userRole(): string | null {
    return this.currentUserValue ? this.currentUserValue.role : null;
  }

  login(username: string, password: string): Observable<UserSession> {
    return this.api.post<UserSession>('auth/login', { username, password }).pipe(
      map(session => {
        localStorage.setItem('leave_track_session', JSON.stringify(session));
        this.currentUserSubject.next(session);
        return session;
      })
    );
  }

  logout(): void {
    localStorage.removeItem('leave_track_session');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  hasRole(roles: string[]): boolean {
    const role = this.userRole;
    return role ? roles.includes(role) : false;
  }

  getToken(): string | null {
    return this.currentUserValue ? this.currentUserValue.token : null;
  }

  forgotPassword(username: string): Observable<{message: string, simulation_token?: string}> {
    return this.api.post<{message: string, simulation_token?: string}>('auth/forgot-password', { username });
  }

  sendForgotPasswordOtp(username: string): Observable<{message: string}> {
    return this.api.post<{message: string}>('auth/send-forgot-password-otp', { username });
  }

  resendForgotPasswordOtp(username: string): Observable<{message: string}> {
    return this.api.post<{message: string}>('auth/resend-forgot-password-otp', { username });
  }

  verifyForgotPasswordOtp(username: string, otp: string): Observable<{message: string, token: string}> {
    return this.api.post<{message: string, token: string}>('auth/verify-forgot-password-otp', { username, otp });
  }

  resetPassword(token: string, newPassword: string): Observable<{message: string}> {
    return this.api.post<{message: string}>('auth/reset-password', { token, newPassword });
  }
}

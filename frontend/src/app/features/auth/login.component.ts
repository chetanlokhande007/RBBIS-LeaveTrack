import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm: FormGroup;
  clockTime = '12:00';
  private clockInterval: any;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private notification: NotificationService
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.updateClock();
    this.clockInterval = setInterval(() => this.updateClock(), 1000);
    
    // Redirect if already logged in
    if (this.authService.isLoggedIn) {
      this.redirectByRole();
    }
  }

  ngOnDestroy() {
    if (this.clockInterval) {
      clearInterval(this.clockInterval);
    }
  }

  updateClock() {
    const now = new Date();
    this.clockTime = now.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    });
  }

  login() {
    if (this.loginForm.invalid) return;

    this.loading = true;
    const { username, password } = this.loginForm.value;

    this.authService.login(username, password).subscribe({
      next: () => {
        this.loading = false;
        this.notification.show('Logged in successfully', 'success');
        this.redirectByRole();
      },
      error: (err) => {
        this.loading = false;
        this.notification.show('Invalid username or password', 'error');
      }
    });
  }


  private redirectByRole() {
    const role = this.authService.userRole;
    if (role === 'Admin') {
      this.router.navigate(['/admin/dashboard']);
    } else if (role === 'Manager') {
      this.router.navigate(['/manager/dashboard']);
    } else if (role === 'Employee') {
      this.router.navigate(['/employee/dashboard']);
    } else if (role === 'HR') {
      this.router.navigate(['/admin/employees']);
    }
  }
}

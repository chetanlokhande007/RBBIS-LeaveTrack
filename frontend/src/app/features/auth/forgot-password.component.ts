import { Component, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { Subscription, interval } from 'rxjs';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnDestroy {
  forgotForm: FormGroup;
  otpForm: FormGroup;
  resetForm: FormGroup;
  
  step: 'username' | 'otp' | 'reset' = 'username';
  loading = false;
  
  countdown = 0; // 60s resend timer
  expiryTimeRemaining = 0; // 5-minute expiry timer
  private timerSub?: Subscription;
  private expiryTimerSub?: Subscription;

  showPassword = false;
  showConfirmPassword = false;

  resetToken: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private notification: NotificationService
  ) {
    this.forgotForm = this.fb.group({
      username: ['', Validators.required] // Accepts both username or email
    });

    this.otpForm = this.fb.group({
      otp: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]]
    });

    this.resetForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  private passwordMatchValidator(form: FormGroup) {
    const p1 = form.get('newPassword')?.value;
    const p2 = form.get('confirmPassword')?.value;
    return p1 === p2 ? null : { mismatch: true };
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  ngOnDestroy() {
    this.stopTimer();
    this.stopExpiryTimer();
  }

  submitUsername() {
    if (this.forgotForm.invalid) return;

    this.loading = true;
    const { username } = this.forgotForm.value;

    this.authService.sendForgotPasswordOtp(username).subscribe({
      next: (response) => {
        this.loading = false;
        this.step = 'otp';
        this.startTimer();
        this.startExpiryTimer();
        this.notification.show('Forgot Password OTP sent successfully.', 'success');
      },
      error: (err) => {
        this.loading = false;
        this.notification.show('Failed to process request', 'error');
      }
    });
  }

  resendOtp() {
    if (this.countdown > 0) return;

    this.loading = true;
    const { username } = this.forgotForm.value;

    this.authService.resendForgotPasswordOtp(username).subscribe({
      next: (response) => {
        this.loading = false;
        this.startTimer();
        this.startExpiryTimer();
        this.notification.show('OTP resent successfully.', 'success');
      },
      error: (err) => {
        this.loading = false;
        this.notification.show('Failed to resend OTP', 'error');
      }
    });
  }

  verifyOtp() {
    if (this.otpForm.invalid) return;

    this.loading = true;
    const { username } = this.forgotForm.value;
    const { otp } = this.otpForm.value;

    this.authService.verifyForgotPasswordOtp(username, otp).subscribe({
      next: (response) => {
        this.loading = false;
        this.notification.show('OTP verified successfully.', 'success');
        this.resetToken = response.token;
        this.stopExpiryTimer();
        this.stopTimer();
        this.step = 'reset';
      },
      error: (err) => {
        this.loading = false;
        const msg = err.error?.message || 'Failed to verify OTP';
        this.notification.show(msg, 'error');
      }
    });
  }

  submitResetPassword() {
    if (this.resetForm.invalid) return;

    this.loading = true;
    const { newPassword } = this.resetForm.value;

    this.authService.resetPassword(this.resetToken, newPassword).subscribe({
      next: (response) => {
        this.loading = false;
        this.notification.show('Password reset successfully.', 'success');
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        this.loading = false;
        const msg = err.error?.message || 'Password reset failed.';
        this.notification.show(msg, 'error');
      }
    });
  }

  get formattedExpiry(): string {
    const m = Math.floor(this.expiryTimeRemaining / 60);
    const s = this.expiryTimeRemaining % 60;
    return `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
  }

  private startExpiryTimer() {
    this.expiryTimeRemaining = 300; // 5 minutes
    this.stopExpiryTimer();
    this.expiryTimerSub = interval(1000).subscribe(() => {
      if (this.expiryTimeRemaining > 0) {
        this.expiryTimeRemaining--;
      } else {
        this.stopExpiryTimer();
      }
    });
  }

  private stopExpiryTimer() {
    if (this.expiryTimerSub) {
      this.expiryTimerSub.unsubscribe();
      this.expiryTimerSub = undefined;
    }
  }

  private startTimer() {
    this.countdown = 60;
    this.stopTimer();
    this.timerSub = interval(1000).subscribe(() => {
      if (this.countdown > 0) {
        this.countdown--;
      } else {
        this.stopTimer();
      }
    });
  }

  private stopTimer() {
    if (this.timerSub) {
      this.timerSub.unsubscribe();
      this.timerSub = undefined;
    }
  }
}

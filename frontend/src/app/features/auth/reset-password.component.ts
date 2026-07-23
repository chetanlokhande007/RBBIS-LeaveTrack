import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './reset-password.component.html'
})
export class ResetPasswordComponent implements OnInit {
  resetForm: FormGroup;
  loading = false;
  submitted = false;
  token = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private notification: NotificationService
  ) {
    this.resetForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit() {
    this.token = this.route.snapshot.queryParams['token'];
    if (!this.token) {
      this.notification.show('Invalid or missing reset token', 'error');
      this.router.navigate(['/login']);
    }
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('newPassword')?.value === g.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  submit() {
    if (this.resetForm.invalid) return;

    this.loading = true;
    const { newPassword } = this.resetForm.value;

    this.authService.resetPassword(this.token, newPassword).subscribe({
      next: () => {
        this.loading = false;
        this.submitted = true;
        this.notification.show('Password successfully reset', 'success');
        
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        this.loading = false;
        this.notification.show(err.error?.message || 'Failed to reset password. The token may be expired.', 'error');
      }
    });
  }
}

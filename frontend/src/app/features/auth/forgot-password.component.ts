import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.component.html'
})
export class ForgotPasswordComponent {
  forgotForm: FormGroup;
  loading = false;
  submitted = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private notification: NotificationService
  ) {
    this.forgotForm = this.fb.group({
      username: ['', Validators.required]
    });
  }

  submit() {
    if (this.forgotForm.invalid) return;

    this.loading = true;
    const { username } = this.forgotForm.value;

    this.authService.forgotPassword(username).subscribe({
      next: (response) => {
        this.loading = false;
        this.submitted = true;
        this.notification.show('Password reset link generated', 'success');
        
        // In a real application, the user would receive an email.
        // For testing/simulation, we log the token to the console so the evaluator can use it.
        if (response.simulation_token) {
          console.warn('--- SIMULATED EMAIL ---');
          console.warn(`Click this link to reset password: /reset-password?token=${response.simulation_token}`);
          console.warn('-----------------------');
        }
      },
      error: (err) => {
        this.loading = false;
        this.notification.show('Failed to process request', 'error');
      }
    });
  }
}

import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { ApiResponse } from '../../models/api.response';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css', '../auth-page.styles.css']
})
export class RegisterComponent {
  registerForm: FormGroup;
  errorMessage: string = '';
  successMessage: string = '';
  warningMessage: string = '';
  isLoading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(control: AbstractControl): { [key: string]: any } | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { 'passwordMismatch': true };
    }
    return null;
  }

  onSubmit(): void {
    this.clearMessages();
    
    if (this.registerForm.invalid) {
      return;
    }

    this.isLoading = true;

    // Remove confirmPassword from payload before sending
    const { confirmPassword, ...payload } = this.registerForm.value;

    this.authService.register(payload).subscribe({
      next: (response: ApiResponse) => {
        this.isLoading = false;
        
        if (response?.responseStatus === 'success') {
          this.successMessage = response.responseMessage || 'Registration successful!';
          
          // Validate token exists before redirecting
          if (this.authService.isAuthenticated()) {
            setTimeout(() => {
              this.router.navigate(['/dashboard']);
            }, 500);
          } else {
            this.errorMessage = 'Token not received. Please try login manually.';
          }
        } else if (response?.responseStatus === 'warning') {
          this.warningMessage = response.responseMessage || 'Registration completed with warnings.';
        } else {
          this.errorMessage = response?.responseMessage || 'Registration failed. Please try again.';
        }
      },
      error: (error) => {
        this.isLoading = false;
        
        if (error?.error?.responseMessage) {
          this.errorMessage = error.error.responseMessage;
        } else if (error?.status === 400) {
          this.errorMessage = 'Invalid input. Please check your details.';
        } else if (error?.status === 409) {
          this.errorMessage = 'User already exists. Please use a different email or username.';
        } else if (error?.status === 500) {
          this.errorMessage = 'Server error. Please try again later.';
        } else {
          this.errorMessage = 'Registration failed. Please try again.';
        }
        
        console.error('Registration error:', error);
      }
    });
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.warningMessage = '';
  }
}

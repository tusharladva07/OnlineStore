import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { ApiResponse } from '../../models/api.response';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css', '../auth-page.styles.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage: string = '';
  successMessage: string = '';
  warningMessage: string = '';
  isLoading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    this.clearMessages();
    
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;

    this.authService.login(this.loginForm.value).subscribe({
      next: (response: ApiResponse) => {
        this.isLoading = false;
        
        if (response?.responseStatus === 'success') {
          this.successMessage = response.responseMessage || 'Login successful!';
          
          // Validate token exists before redirecting
          if (this.authService.isAuthenticated()) {
            setTimeout(() => {
              this.router.navigate(['/dashboard']);
            }, 500);
          } else {
            this.errorMessage = 'Token not received. Please try again.';
          }
        } else if (response?.responseStatus === 'warning') {
          this.warningMessage = response.responseMessage || 'Login completed with warnings.';
        } else {
          this.errorMessage = response?.responseMessage || 'Login failed. Please try again.';
        }
      },
      error: (error) => {
        this.isLoading = false;
        
        if (error?.error?.responseMessage) {
          this.errorMessage = error.error.responseMessage;
        } else if (error?.status === 400) {
          this.errorMessage = 'Invalid email or password.';
        } else if (error?.status === 500) {
          this.errorMessage = 'Server error. Please try again later.';
        } else {
          this.errorMessage = 'Login failed. Please check your credentials.';
        }
        
        console.error('Login error:', error);
      }
    });
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.warningMessage = '';
  }
}

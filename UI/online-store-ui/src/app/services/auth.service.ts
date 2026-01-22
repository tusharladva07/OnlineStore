import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { LoginRequest, RegisterRequest } from '../models/auth.models';
import { ApiResponse, TokenResponse } from '../models/api.response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5286';

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<ApiResponse<TokenResponse>> {
    return this.http.post<ApiResponse<TokenResponse>>(`${this.apiUrl}/User/login`, request).pipe(
      tap(response => {
        this.handleAuthResponse(response);
      }),
      catchError(error => {
        return throwError(() => error);
      })
    );
  }

  register(request: RegisterRequest): Observable<ApiResponse<TokenResponse>> {
    return this.http.post<ApiResponse<TokenResponse>>(`${this.apiUrl}/User/register`, request).pipe(
      tap(response => {
        this.handleAuthResponse(response);
      }),
      catchError(error => {
        return throwError(() => error);
      })
    );
  }

  private handleAuthResponse(response: ApiResponse<TokenResponse>): void {
    if (response && response.responseStatus === 'success' && response.responseObject?.token) {
      localStorage.setItem('authToken', response.responseObject.token);
      if (response.responseObject.username) {
        localStorage.setItem('currentUser', response.responseObject.username);
      }
    }
  }

  logout(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('authToken');
  }

  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  getCurrentUser(): string | null {
    return localStorage.getItem('currentUser');
  }
}
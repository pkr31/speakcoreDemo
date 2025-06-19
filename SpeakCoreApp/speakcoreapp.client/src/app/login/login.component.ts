import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../models/user-model';



@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  loginError = '';
  loginSuccess = false;
  currentUser: any = null;

  constructor(
    private fb: FormBuilder, private http: HttpClient, private router: Router, private authService: AuthService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  ngOnInit(): void { }

  async onLogin() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.loginError = '';
      this.loginSuccess = false;

      try {
        const loginData: LoginRequest = this.loginForm.value;
        const response: any = await firstValueFrom(
          this.authService.login(loginData)
        );

        // Save token to sessionStorage
        if (response?.token) {
          sessionStorage.setItem('token', response.token);

          // Optional: store user info if needed
          const { key, registrationId, firstName, lastName, email } = response;
          sessionStorage.setItem('user', JSON.stringify({ key, registrationId, firstName, lastName, email }));
        }

        this.currentUser = response;
        this.loginSuccess = true;
        this.loginForm.reset();
        this.router.navigate(['/dashboard']);
      } catch (error: any) {
        if (error.status === 401) {
          this.loginError = 'Invalid email or password. Please try again.';
        } else {
          this.loginError = 'Login failed. Please try again later.';
        }
        console.error('Login error:', error);
      } finally {
        this.isLoading = false;
      }
    }
  }
}

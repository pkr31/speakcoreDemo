import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { RegistrationRequest, User } from '../models/user-model';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm: FormGroup;
  isLoading = false;
  registerError = '';
  currentUser: User | null = null;


  states = [
    'Alabama', 'Alaska', 'Arizona', 'Arkansas', 'California', 'Colorado',
    'Connecticut', 'Delaware', 'Florida', 'Georgia', 'Hawaii', 'Idaho',
    'Illinois', 'Indiana', 'Iowa', 'Kansas', 'Kentucky', 'Louisiana',
    'Maine', 'Maryland', 'Massachusetts', 'Michigan', 'Minnesota',
    'Mississippi', 'Missouri', 'Montana', 'Nebraska', 'Nevada',
    'New Hampshire', 'New Jersey', 'New Mexico', 'New York',
    'North Carolina', 'North Dakota', 'Ohio', 'Oklahoma', 'Oregon',
    'Pennsylvania', 'Rhode Island', 'South Carolina', 'South Dakota',
    'Tennessee', 'Texas', 'Utah', 'Vermont', 'Virginia', 'Washington',
    'West Virginia', 'Wisconsin', 'Wyoming'
  ];
  constructor(
    private fb: FormBuilder, private http: HttpClient, private router: Router, private authService: AuthService
  ) {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      state: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      confirmEmail: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
      subscribe: [false]
    }, { validators: this.emailMatchValidator });
  }

  ngOnInit(): void { }

  emailMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const email = control.get('email');
    const confirmEmail = control.get('confirmEmail');

    if (email && confirmEmail && email.value !== confirmEmail.value) {
      return { emailMismatch: true };
    }
    return null;
  }

  async onRegister() {
    if (this.registerForm.valid) {
      this.isLoading = true;
      this.registerError = '';

      try {
        const registrationData: RegistrationRequest = this.registerForm.value;
        const response = await firstValueFrom(
          this.authService.register(registrationData)
        );
        const registeredUser: User = response;
        this.registerForm.reset();
        // Navigate to thank you page with user data
        this.router.navigate(['/thank-you'], {
          state: { registeredUser }
        });
      } catch (error: any) {
        if (error.status === 409) {
          this.registerError = 'This email is already registered. Please use a different email or try logging in.';
        } else if (error.status === 400) {
          this.registerError = 'Please check your information and try again.';
        } else {
          this.registerError = 'Registration failed. Please try again later.';
        }
        console.error('Registration error:', error);
      } finally {
        this.isLoading = false;
      }
    }
  }
}

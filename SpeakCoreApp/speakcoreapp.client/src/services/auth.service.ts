import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { Router } from '@angular/router';
import { User } from '../app/models/user-model';
import Swal from 'sweetalert2';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private apiUrl = environment.apiBaseUrl;

    constructor(private http: HttpClient, private router: Router) { }

    login(data: any) {
        return this.http.post(`${this.apiUrl}/registration/login`, data);
    }

    register(data: any) {
        return this.http.post<User>(`${this.apiUrl}/registration/register`, data)
    }

    logout() {
        sessionStorage.clear();
        this.router.navigate(['/']);
    }

  deleteAccount() {
        Swal.fire({
          title: "Are you sure?",
          text: "You won't be able to revert this!",
          icon: "warning",
          showCancelButton: true,
          confirmButtonText: "Yes, delete it!",
          buttonsStyling: false,
          customClass: {
            confirmButton: 'btn btn-primary px-4',
            cancelButton: 'btn btn-danger ms-2 px-4',
          },
        }).then((result) => {
          if (result.value) {
            const userJson = sessionStorage.getItem('user');
            const token = sessionStorage.getItem('token');
            const user: any = userJson ? JSON.parse(userJson) : null;
            if (!user.registrationId || !token) {
              Swal.fire({
                text: 'User ID or token not found.',
                customClass: {
                  confirmButton: 'btn btn-primary px-4',
                },
                buttonsStyling: false,
              });
              return;
            }

            this.http.delete(`${this.apiUrl}/registration/${user.registrationId}`, {
              headers: { Authorization: `Bearer ${token}` }
            }).subscribe({
              next: () => {
                Swal.fire({
                  title: 'Deleted!',
                  text: 'Account Deleted successfully.!',
                  icon: 'success',
                  showCancelButton: false,
                  confirmButtonText: 'OK',
                  buttonsStyling: false,

                  customClass: {
                    confirmButton: 'btn btn-primary px-4',
                    cancelButton: 'btn btn-danger ms-2 px-4',

                  },
                });
                sessionStorage.clear();
                this.router.navigate(['/login']);
              },
              error: () => {
                Swal.fire({
                  title: 'Something went wrong!',
                  text: 'Failed to delete account. Please try again.',
                  icon: 'error',
                  showCancelButton: false,
                  confirmButtonText: 'OK',
                  buttonsStyling: false,

                  customClass: {
                    confirmButton: 'btn btn-primary px-4'
                  },
                });
              }
            });
          }
         
         
        });
    }



}

import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  constructor(private http: HttpClient, private router: Router, private authService: AuthService) { }

  ngOnInit(): void {
    const token = sessionStorage.getItem('token');

    if (!token) {
      alert('Unauthorized! Please login.');
      this.router.navigate(['/']);
    }
  }

  logout(): void {
    this.authService.logout();
  }

  deleteAccount(): void {
    this.authService.deleteAccount();
  }
}

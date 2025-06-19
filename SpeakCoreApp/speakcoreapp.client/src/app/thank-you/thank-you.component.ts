import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-thank-you',
  templateUrl: './thank-you.component.html',
  styleUrl: './thank-you.component.css'
})
export class ThankYouComponent {
  registeredUser: any = null;

  constructor(private router: Router) {
    // Get the user data from navigation state
    const navigation = this.router.getCurrentNavigation();
    if (navigation?.extras?.state) {
      this.registeredUser = navigation.extras.state['registeredUser'];
    }
  }

  ngOnInit(): void {
    // If no user data is available, redirect to register
    if (!this.registeredUser) {
      this.router.navigate(['/register']);
    }
  }

  startOver(): void {
    this.router.navigate(['/register']);
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}

import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const token = sessionStorage.getItem('token');
  const router = inject(Router);
  if (token) {
    return true;
  } else {
    alert('You must be logged in to access this page.');
    return router.parseUrl('/');
  }
};

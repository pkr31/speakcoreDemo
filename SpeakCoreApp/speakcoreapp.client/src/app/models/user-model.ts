
export interface LoginRequest {
  registrationId: string;
  firstName: string;
  lastName: string;
  email: string;
}

export interface RegistrationRequest {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
}

export interface User {
    key: string;
    registrationId: string;
    firstName: string;
    lastName: string;
    email: string;
}

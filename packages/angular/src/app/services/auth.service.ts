import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';

export interface IUser {
  email: string;
  name?: string;
  avatarUrl?: string;
}

export interface IResponse {
  isOk: boolean;
  data?: IUser;
  message?: string;
}

const defaultPath = '/';
export const defaultUser: IUser = {
  email: 'jheart@dx-email.com',
  name: 'John Heart',
  avatarUrl: 'https://js.devexpress.com/Demos/WidgetsGallery/JSDemos/images/employees/01.png',
};

@Injectable()
export class AuthService {
  private _user: IUser | null = null;

  get loggedIn(): boolean {
    return !!this._user;
  }

  private _lastAuthenticatedPath: string = defaultPath;

  get lastAuthenticatedPath(): string {
    return this._lastAuthenticatedPath;
  }
  set lastAuthenticatedPath(value: string) {
    this._lastAuthenticatedPath = value;
  }

  constructor(private http: HttpClient,private router: Router) { }

  // async logIn(username: string, password: string) {
  //   try {
  //     // Send request
  //     this.router.navigate([this._lastAuthenticatedPath]);

  //     return {
  //       isOk: true,
  //       data: this._user,
  //     };
  //   } catch {
  //     return {
  //       isOk: false,
  //       message: 'Authentication failed',
  //     };
  //   }
  // }

  logIn(username: string, password: string) : Observable<any> {
      const user = {username: username, password: password}
      return this.http.post<any>('https://localhost:7062/api/Users/login', user)

  }


  getUser() : Observable<any> {
    // Send request
    return this.http.get<any>('https://localhost:7062/api/Users/UserLoggedIn', { withCredentials: true });
  }

  async createAccount(email: string, password: string) {
    try {
      // Send request

      this.router.navigate(['/auth/create-account']);
      return {
        isOk: true,
      };
    } catch {
      return {
        isOk: false,
        message: 'Failed to create account',
      };
    }
  }

  async changePassword(email: string, recoveryCode: string) {
    try {
      // Send request

      return {
        isOk: true,
      };
    } catch {
      return {
        isOk: false,
        message: 'Failed to change password',
      };
    }
  }

  async resetPassword(email: string) {
    try {
      // Send request

      return {
        isOk: true,
      };
    } catch {
      return {
        isOk: false,
        message: 'Failed to reset password',
      };
    }
  }

  async logOut() {
    this.router.navigate(['/auth/login']);
  }
}

@Injectable()
export class AuthGuardService implements CanActivate {
  constructor(private router: Router, private authService: AuthService) { }

    canActivate(route: ActivatedRouteSnapshot): boolean {
      const isLoggedIn = this.authService.loggedIn;
      const isAuthForm = [
        'login',
        'reset-password',
        'create-account',
        'change-password/:recoveryCode',
      ].includes(route.routeConfig?.path || defaultPath);

      if (!isLoggedIn && !isAuthForm) {
        this.router.navigate(['/auth/login']);
      }

      if (isLoggedIn) {
        this.authService.lastAuthenticatedPath = route.routeConfig?.path || defaultPath;
      }

      return isLoggedIn || isAuthForm;
    }
}

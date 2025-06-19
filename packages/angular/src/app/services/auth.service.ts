import { map, Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { IApiResponseData } from '../core/models/response/response';
import { IUserDto } from '../core/models/user/user-dto';

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
  private _user: IUserDto | null = null;

  get loggedIn(): boolean {
    const userJson = localStorage.getItem('currentUser');
    this._user = JSON.parse(userJson) as IUserDto;
    return !!this._user;
  }

  private _lastAuthenticatedPath: string = defaultPath;

  get lastAuthenticatedPath(): string {
    return this._lastAuthenticatedPath;
  }
  set lastAuthenticatedPath(value: string) {
    this._lastAuthenticatedPath = value;
  }

  constructor(private http: HttpClient, private router: Router) { }

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

  logIn(username: string, password: string) : Observable<IApiResponseData<IUserDto>> {
      const user = {username: username, password: password}
      return this.http.post<IApiResponseData<IUserDto>>('https://localhost:7062/api/Users/login', user)
      .pipe(
        tap(response=> {
          if (response && response.data) {
            this._user = response.data;
            if(!this._user.imageUrl){
              this._user.imageUrl = 'https://js.devexpress.com/Demos/WidgetsGallery/JSDemos/images/employees/01.png'
            }
            // También puedes almacenar en localStorage si necesitas persistencia
            localStorage.setItem('currentUser', JSON.stringify(response.data));
          }
        })
      );

  }


  getUser() : Observable<any> {
    // Send request
    return this.http.get<any>('https://localhost:7062/api/Users/UserLoggedIn');
    // return this.http.get<any>('https://localhost:7062/api/Users/debug-auth');
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

    try {
    // 1. Llamar al endpoint de logout del servidor (si existe)
      await this.http.post('https://localhost:7062/api/Users/logout', {}).toPromise();
    } catch (error) {
      console.log('Error en logout del servidor:', error);
    }

    this.clearAuthCookie();
    this.clearSessionData();
  }

  private clearAuthCookie() {
  console.log('Cookies antes del logout:', document.cookie);

  // Borrar AuthToken con diferentes combinaciones para asegurar que se elimine
  document.cookie = 'AuthToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
  document.cookie = `AuthToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; domain=${window.location.hostname};`;
  document.cookie = `AuthToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; domain=.${window.location.hostname};`;

  // Si usas HTTPS
  document.cookie = 'AuthToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; secure;';
  document.cookie = `AuthToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; domain=${window.location.hostname}; secure;`;

  setTimeout(() => {
    console.log('Cookies después del logout:', document.cookie);
  }, 100);
}

private clearSessionData() {
  // Limpiar localStorage
  localStorage.clear();

  // O limpiar keys específicas
  // localStorage.removeItem('userProfile');
  // localStorage.removeItem('permissions');

  // Limpiar sessionStorage
  sessionStorage.clear();

  // Limpiar variables del servicio
  // this.currentUser = null;
  // this.isAuthenticated = false;
  // Agrega otras variables que uses para el estado de autenticación
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

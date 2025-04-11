import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private router: Router) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    console.log('Interceptor ejecutándose:', request.url);

    const excludedBaseUrl = 'https://js.devexpress.com/Demos/RwaService/api';

    if (request.url.startsWith(excludedBaseUrl)) {
      // No modificar esta petición, dejarla pasar tal cual
      return next.handle(request);
    }
    // Obtener token del localStorage
    const token = localStorage.getItem('token');

    // Clonar la petición para añadir withCredentials y/o el token
    request = request.clone({
      withCredentials: true, // Esto permite enviar cookies en peticiones cross-origin
    });

    // Si también hay token en localStorage, añadirlo al header
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        },
        withCredentials: true
      });
    }

    // Continuar con la solicitud y manejar errores
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        // Si el error es 401 (no autorizado), redirigir al login
        if (error.status === 401) {
          localStorage.removeItem('token');
          this.router.navigate(['/login']);
        }
        return throwError(() => error);
      })
    );
  }
}

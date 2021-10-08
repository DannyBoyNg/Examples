import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError, of, TimeoutError } from 'rxjs';

import { AuthService } from './auth.service';
import { catchError, switchMap, filter, take, finalize } from 'rxjs/operators';
import { Token } from './auth.interfaces';
import { DialogService } from '@dannyboyng/dialog';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  private isRefreshing = false;
  private mostRecentTokenSource = new BehaviorSubject<any>(null);

  constructor(private auth: AuthService, private dialog: DialogService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.auth.getAccessToken();
    const newReq = (token != null) ? this.addTokenToRequest(req, token) : req;
    return next.handle(newReq).pipe(
      catchError(err => {
        // Handle access token expired error
        const t = err.headers.get('WWW-Authenticate');
        if (t != null && t.includes('token expired')) {
          return this.handleExpiredAccessToken(req, next);
        }
        // Run Global error handler
        this.globalErrorsHandler(err);
        return throwError(err);
      })
    );
  }

  private addTokenToRequest(request: HttpRequest<any>, token: string) {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  private handleExpiredAccessToken(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const accessToken = this.auth.getAccessToken();
    const refreshToken = this.auth.getRefreshToken();
    if (accessToken == null || refreshToken == null) { return of(); }
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.mostRecentTokenSource.next(null);
      return this.auth.refreshAccessToken(accessToken, refreshToken).pipe(
        finalize(() => this.isRefreshing = false),
        switchMap(token => {
          this.mostRecentTokenSource.next(token);
          return next.handle(this.addTokenToRequest(request, token.accessToken));
        }));
    } else {
      return this.mostRecentTokenSource.pipe(
        filter(token => token != null),
        take(1),
        switchMap((token: Token) => {
          return next.handle(this.addTokenToRequest(request, token.accessToken));
        }),
      );
    }
  }

  // Main purpose of global error handler is to communicate error to the user
  private globalErrorsHandler(err: HttpErrorResponse) {
    let message: string|string[];
    if (err instanceof TimeoutError) {
      // Handle the error thrown by the RxJs timeout operator
      message = 'A timeout error occured';
    } else if (err.error instanceof Error) {
      // A client-side or network error occurred. Handle it accordingly.
      message = err.error.message;
    } else {
      // The backend returned an unsuccessful response code.
      switch (err.status) {
        case 0: {
          // Handle connection errors
          message = 'An connection error or an server error has occured. There could be a problem with your connection.';
          break;
        }
        case 400: {
          // Handle bad request errors
          message = err.error;
          // Handle file request/response related errors
          if (message instanceof Blob) {
            const reader = new FileReader();
            reader.readAsText(message);
            reader.onload = () => {
              message = (reader.result != null) ? reader.result.toString() : '';
              this.dialog.error(message);
            };
            return;
          }
          break;
        }
        case 401: {
          // Handle invalid token errors
          if (err.error === 'Invalid refresh token' || err.error === 'Invalid access token') {
            return;
          }
          // Handle not authorized errors
          message = 'You are not authorized to perform this action';
          break;
        }
        case 404: {
          // Handle not found errors
          message = [err.url || '', 'Not found'];
          break;
        }
        case 409: {
          // Handle conflict errors. Global error handler doesn't have to do anything. Let the caller handle the error.
          return;
        }
        case 415: {
          // Handle unsupported media errors
          message = ['Api Error 415: Unsupported Media Type', err.url || ''];
          break;
        }
        default: {
          // Handle other errors
          message = `Error: ${err.statusText}.`;
        }
      }
    }
    this.dialog.error(message);
  }

}

import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';
import { Token } from './auth.interfaces';

@Injectable({
  providedIn: 'root',
})
export class AuthApiService {

  private api = environment.apiUrl;

  constructor(
    private http: HttpClient,
  ) { }

  login(username: string, password: string) {
    const params = new HttpParams()
    .set('username', username.toString())
    .set('password', password.toString());
    return this.http.post<Token>(`${this.api}/token`, null, {params});
  }

  refreshAccessToken(accesToken: string, refreshToken: string) {
    const params = new HttpParams()
    .set('accessToken', accesToken)
    .set('refreshToken', refreshToken);
    return this.http.post<Token>(`${this.api}/refresh`, null, {params});
  }

  register(username: string, email: string, password: string) {
    const params = new HttpParams()
    .set('username', username.toString())
    .set('email', email.toString())
    .set('password', password.toString());
    return this.http.post(`${this.api}/api/user/register`, null, {params});
  }

  confirmEmail(userId: number, token: string) {
    return this.http.get(`${this.api}/api/user/confirmemail/${userId}/${token}`);
  }

  forgotPassword(email: string) {
    const params = new HttpParams()
    .set('email', email);
    return this.http.post(`${this.api}/api/user/forgotpassword`, null, {params});
  }

  resetPassword(userId: string, token: string, password: string, confirmPassword: string) {
    const params = new HttpParams()
    .set('userid', userId)
    .set('token', token)
    .set('password', password)
    .set('confirmpassword', confirmPassword);
    return this.http.post(`${this.api}/api/user/resetpassword`, null, {params});
  }

  forgotUsername(email: string) {
    const params = new HttpParams()
    .set('email', email);
    return this.http.post(`${this.api}/api/user/forgotusername`, null, {params});
  }
}

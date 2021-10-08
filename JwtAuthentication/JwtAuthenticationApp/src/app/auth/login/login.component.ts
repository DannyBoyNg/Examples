import { Router } from '@angular/router';
import { Component, OnInit, ElementRef, ViewChild, AfterViewInit } from '@angular/core';

import { AuthApiService } from '../auth-api.service';
import { AuthService } from '../auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { finalize, tap } from 'rxjs/operators';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, AfterViewInit {

  form: FormGroup;
  loginInProgress = false;
  @ViewChild('uiUsername', { static: true }) uiUsername!: ElementRef;

  constructor(
    private auth: AuthService,
    private api: AuthApiService,
    private fb: FormBuilder,
    private router: Router,
  ) {
    this.form = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  ngOnInit() {
    if (this.auth.isLoggedIn() === true) {
      this.router.navigate(['']);
      return;
    }
  }

  ngAfterViewInit() {
    this.uiUsername.nativeElement.focus();
  }

  login() {
    this.loginInProgress = true;
    const form = this.form.value;
    const username = form.username;
    const password = form.password;
    this.api.login(username, password)
      .pipe(
        finalize(() => this.loginInProgress = false),
        tap(token => {
          // store token
          this.auth.setToken(token);
          // redirect after login
          if (this.auth.redirectUrl != null) {
            this.router.navigateByUrl(this.auth.redirectUrl);
          } else {
            this.router.navigate(['']);
          }
        })
      )
      .subscribe();
  }
}

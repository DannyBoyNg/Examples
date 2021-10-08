import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { DialogService } from '@dannyboyng/dialog';
import { map } from 'rxjs/operators';

import { AuthApiService } from '../auth-api.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {

  errors: string[] = [];
  form: FormGroup;
  private userId: string|undefined;
  private token: string|undefined;

  constructor(
    private api: AuthApiService,
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private dialog: DialogService,
  ) {
    this.form = this.fb.group({
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required],
    });
  }

  ngOnInit() {
    this.route.paramMap.pipe(
      map((params: ParamMap) => {
        this.userId = params.get('userid') || '';
        this.token = params.get('token') || '';
        if (!this.userId || !this.token) {this.router.navigateByUrl('/login'); }
      })
    ).subscribe();
  }

  onSubmit() {
    this.errors = [];
    if (this.form == null) {return; }
    const data = this.form.value;
    // validate password
    if (data.password.length < 8) {
      setTimeout(() => {
        if (this.errors != null) {this.errors.push('Password needs to be at least 8 characters.'); }
      }, 200);
      return;
    }
    if (data.password !== data.confirmPassword) {
      setTimeout(() => {
        if (this.errors != null) {this.errors.push('Password and Confirm password are not equal.'); }
      }, 200);
      return;
    }
    if (!this.userId || !this.token) {return; }
    this.api.resetPassword(this.userId, this.token, data.password, data.confirmPassword)
    .subscribe(() => {
      this.dialog.info('Your password has been reset')
      .subscribe(() => this.router.navigateByUrl('/login'));
    });
  }

}

import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DialogService } from '@dannyboyng/dialog';

import { AuthApiService } from '../auth-api.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit {

  email: string|undefined;
  errors: string[]|undefined;
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private api: AuthApiService,
    private dialog: DialogService,
    private router: Router,
  ) {
    this.form = this.fb.group({
      email: ['', Validators.required],
    });
  }

  ngOnInit() {
  }

  onSubmit() {
    if (this.form == null) {return; }
    const data = this.form.value;
    this.api.forgotPassword(data.email)
    .subscribe(() => {
      this.dialog.info('Check your mail to find instructions on how to reset your password')
      .subscribe(() => this.router.navigateByUrl('/login'));
    });
  }

}

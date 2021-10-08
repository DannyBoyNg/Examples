import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DialogService } from '@dannyboyng/dialog';

import { AuthApiService } from '../auth-api.service';

@Component({
  selector: 'app-forgot-username',
  templateUrl: './forgot-username.component.html',
  styleUrls: ['./forgot-username.component.css']
})
export class ForgotUsernameComponent implements OnInit {

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
    this.api.forgotUsername(data.email)
    .subscribe(() => {
      this.dialog.info('Your username is sent to your email address.')
      .subscribe(() => this.router.navigateByUrl('/login'));
    });
  }
}

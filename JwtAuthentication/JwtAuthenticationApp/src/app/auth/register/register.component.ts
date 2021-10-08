import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DialogService } from '@dannyboyng/dialog';

import { AuthApiService } from '../auth-api.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {

  registerInProgress = false;
  username: string|undefined;
  email: string|undefined;
  password: string|undefined;
  confirmPassword: string|undefined;
  errors: string[]|undefined;
  submitted = false;
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private api: AuthApiService,
    private dialog: DialogService,
    private router: Router,
  ) {
    this.form = this.fb.group({
      username: ['', Validators.required],
      email: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required],
    });
  }

  ngOnInit() {
  }

  onSubmit() {
    this.errors = [];
    if (this.form == null) {return; }
    const data = this.form.value;
    // Validate password
    if (data.password !== data.confirmPassword) {
      setTimeout(() => {
        if (this.errors != null) {this.errors.push('Password and Confirm password are not equal.'); }
      }, 200);
      return;
    }
    // Send to server
    this.registerInProgress = true;
    this.api.register(data.username, data.email, data.password)
    .pipe(finalize(() => this.registerInProgress = false))
    .subscribe(() => {
      this.dialog.info('Your account has been created').subscribe(() => this.router.navigateByUrl('/login'));
    });
  }

}

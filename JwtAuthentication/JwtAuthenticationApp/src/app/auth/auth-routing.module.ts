import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { ForgotUsernameComponent } from './forgot-username/forgot-username.component';

const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'register',
    component: RegisterComponent,
  },
  {
    path: 'confirmemail/:userid/:token',
    component: ConfirmEmailComponent,
  },
  {
    path: 'forgotpassword',
    component: ForgotPasswordComponent,
  },
  {
    path: 'resetpassword/:userid/:token',
    component: ResetPasswordComponent,
  },
  {
    path: 'forgotusername',
    component: ForgotUsernameComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }

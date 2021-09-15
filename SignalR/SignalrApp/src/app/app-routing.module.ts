import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SignalrNoAuthenticationComponent } from './pages/signalr-no-authentication/signalr-no-authentication.component';
import { SignalrWithJwtAuthenticationComponent } from './pages/signalr-with-jwt-authentication/signalr-with-jwt-authentication.component';
import { SignalrWithWindowsAuthenticationComponent } from './pages/signalr-with-windows-authentication/signalr-with-windows-authentication.component';

const routes: Routes = [
  { path: 'signalr-no-authentication', component: SignalrNoAuthenticationComponent },
  { path: 'signalr-windows-authentication', component: SignalrWithWindowsAuthenticationComponent },
  { path: 'signalr-jwt-authentication', component: SignalrWithJwtAuthenticationComponent },
  {
    path: '',
    redirectTo: '/signalr-no-authentication',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: '/signalr-no-authentication',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

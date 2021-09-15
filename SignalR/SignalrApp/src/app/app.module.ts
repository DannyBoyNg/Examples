import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SignalrWithWindowsAuthenticationComponent } from './pages/signalr-with-windows-authentication/signalr-with-windows-authentication.component';
import { SignalrWithJwtAuthenticationComponent } from './pages/signalr-with-jwt-authentication/signalr-with-jwt-authentication.component';
import { SignalrNoAuthenticationComponent } from './pages/signalr-no-authentication/signalr-no-authentication.component';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [
    AppComponent,
    SignalrWithWindowsAuthenticationComponent,
    SignalrWithJwtAuthenticationComponent,
    SignalrNoAuthenticationComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

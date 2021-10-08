import { Component, OnInit, AfterViewInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';

import { AuthApiService } from '../auth-api.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit, AfterViewInit {

  response = '';

  constructor(
    private api: AuthApiService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {}

  ngAfterViewInit() {
    this.route.paramMap.pipe(
      switchMap((params: ParamMap) => {
        const userId = params.get('userid') || 0;
        const token = params.get('token') || '';
        return this.api.confirmEmail(+userId, token);
      }),
      catchError((err) => {
        this.response = `${err.error}`;
        return throwError(err);
      })
    ).subscribe(() => this.response = `Your email has been succesfully confirmed`);
  }

}

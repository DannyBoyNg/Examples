import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoadingBarService } from '@dannyboyng/loading-bar';
import { finalize } from 'rxjs/operators';

@Injectable()
export class ApiLoadingInterceptor implements HttpInterceptor {

  constructor(private loader: LoadingBarService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.loader.show();
    return next.handle(req)
    .pipe(finalize(() => this.loader.hide()));
  }

}

import { Component, ViewContainerRef } from '@angular/core';
import { DialogService } from '@dannyboyng/dialog';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  constructor(
    private vcr: ViewContainerRef,
    private dialog: DialogService
  ) {
    this.dialog.setViewContainerRef(this.vcr);
  }
}

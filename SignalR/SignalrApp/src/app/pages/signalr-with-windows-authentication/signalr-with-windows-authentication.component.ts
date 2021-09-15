import { Component, OnDestroy, OnInit } from '@angular/core';
import { DialogService } from '@dannyboyng/dialog';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { ChatMessage } from 'src/app/models/chatmessage.interface';

@Component({
  selector: 'app-signalr-with-windows-authentication',
  templateUrl: './signalr-with-windows-authentication.component.html',
  styleUrls: ['./signalr-with-windows-authentication.component.css']
})
export class SignalrWithWindowsAuthenticationComponent implements OnDestroy {

  HubConnection: HubConnection|undefined;
  userInput: string = '';
  signalrServer: string = 'https://localhost:44324/chat';
  chatBuffer: ChatMessage[] = [];

  constructor(private dialog: DialogService) {}

  ngOnDestroy(): void {
    this.disconnect();
  }

  connect() {
    // Build SignalR connection
    this.HubConnection = new HubConnectionBuilder()  
    .configureLogging(LogLevel.Debug)  
    .withUrl(this.signalrServer)  
    .build();

    // Local copy
    const connection = this.HubConnection;

    connection
    .start()
    .then(function () {  
      console.log('SignalR Connected!');
    })
    .catch(err => {
      this.dialog.error(`Unable to connect: ${err}`);
    });

    // On close handler
    connection.onclose(err => {
      if (err != null) this.dialog.error(err);
    });
  
    // Listen for incoming data
    connection.on('BroadcastMessage', (m: ChatMessage) => {
      if (m.timeStamp != null) m.dateTime = new Date(m.timeStamp);
      this.chatBuffer.push(m);
    });
  }

  disconnect() {
    const connection = this.HubConnection;
    if (connection == null) return;
    connection.off('BroadcastMessage');
    connection.stop();
  }

  sendToServer() {
    const message: ChatMessage = {
      message: this.userInput,
    };
    const connection = this.HubConnection;
    if (connection != null) {
      connection.invoke('SubmitMessage', message)
      .catch(err => this.dialog.error(err));
    }

    this.userInput = '';
  }

}

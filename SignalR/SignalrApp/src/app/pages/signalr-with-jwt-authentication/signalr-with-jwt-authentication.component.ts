import { Component, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DialogService } from '@dannyboyng/dialog';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { ChatMessage } from 'src/app/models/chatmessage.interface';

@Component({
  selector: 'app-signalr-with-jwt-authentication',
  templateUrl: './signalr-with-jwt-authentication.component.html',
  styleUrls: ['./signalr-with-jwt-authentication.component.css']
})
export class SignalrWithJwtAuthenticationComponent implements OnDestroy {

  HubConnection: HubConnection|undefined;
  signalrServer: string = 'https://localhost:44338/chat';
  tokenServer: string = 'https://localhost:44338/token';
  userInput: string = '';
  userName: string = '';
  chatBuffer: ChatMessage[] = [];
  accessToken: string = '';

  constructor(
    private dialog: DialogService,
    private http: HttpClient,
  ) {}

  ngOnDestroy(): void {
    this.disconnect();
  }

  connect() {
    // Build SignalR connection
    this.HubConnection = new HubConnectionBuilder()  
    .configureLogging(LogLevel.Debug)  
    .withUrl(`${this.signalrServer}`, { accessTokenFactory: () => this.accessToken })  
    .build();

    // Local copy
    const connection = this.HubConnection;

    connection
    .start()
    .then(function () {  
      console.log('SignalR Connected!');
    })
    .catch((err: any) => {
      this.dialog.error(`Unable to connect: ${err}`);
    });  

    // On close handler
    connection.onclose(err => {
      if (err != null) this.dialog.error(err);
    });
    
    // Listen for incoming data
    connection.on('BroadcastMessage', (m: ChatMessage) => {
      if (m.timeStamp != null) m.dateTime = new Date(m.timeStamp);
      this.chatBuffer.push(m)
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
      userName: this.userName
    };
    const connection = this.HubConnection;

    if (connection != null) {
      connection.invoke('SubmitMessage', message)
      .catch(err => this.dialog.error(`${err}`));
    }
    
    this.userInput = '';
  }

  getToken() {
    if (!this.userName) {
      this.dialog.error('You must provide an username first');
      return;
    }
    const args: GetTokenArgs = {
      UserName: this.userName
    }
    this.http.post(`${this.tokenServer}`, args, {responseType: 'text'})
    .subscribe(x => this.accessToken = x);
  }
}

interface GetTokenArgs {
  UserName: string,
  Roles?: string[],
  UserDefinedClaims?: UserDefinedClaims,
}

interface UserDefinedClaims{
  [key: string]: string;
}

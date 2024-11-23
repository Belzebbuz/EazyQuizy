import {Injectable} from '@angular/core';
import {connect, NatsConnection, StringCodec, Subscription} from 'nats.ws';
import {KeycloakService} from 'keycloak-angular';

@Injectable({
  providedIn: 'root'
})
export class NatsService {
  connection?: NatsConnection;

  constructor(private readonly keycloak: KeycloakService) {
  }

  async connect() {
    if(this.connection)
      return;
    if (!this.keycloak.isLoggedIn())
      return;
    this.connection = await connect(
      {
        servers: ["ws://localhost:8080"],

      },
    )
  }

  async* subscribe<T>(topic: string, canConnect?: () => boolean): AsyncGenerator<T> {
    const sub = await this.createSub(topic, canConnect);
    if(!sub) return;
    for await (let msg of sub) {
      try {
        yield msg.json<T>();
      } catch (ex: any) {
        console.log('error while handling ws message')
      }
    }
  }
  async createSub(topic: string, canConnect?: () => boolean): Promise<Subscription | null>{
    if(canConnect && !canConnect())
      return null;
    if (!this.connection)
      await this.connect()
    if(!this.connection)
      return null;
    return this.connection.subscribe(topic)
  }
}
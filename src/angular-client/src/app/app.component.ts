import {Component, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {createChannel, createClientFactory, Metadata} from 'nice-grpc-web';
import {CreateModuleRequest, ModuleServiceClient, ModuleServiceDefinition} from '../generated/modules/module';
import {KeycloakService} from 'keycloak-angular';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'angular-client';
  channel = createChannel("http://127.0.0.1:5530");
  token? : string;
  clientFactory = createClientFactory().use((call, options) =>
    call.next(call.request, {
      ...options,
      metadata: Metadata(options.metadata).set(
        'Authorization',
        `Bearer ${this.token}`,
      ),
    }),
  );
  client: ModuleServiceClient = this.clientFactory.create(ModuleServiceDefinition, this.channel);
  constructor(private readonly keycloak: KeycloakService) {
  }
  async create(){
    let rq = CreateModuleRequest.create();
    rq.name = "value";
    const resp = await this.client.create(rq);
    console.log(resp);
  }
  async ngOnInit()  {
    if (this.keycloak.isLoggedIn()) {
      this.token = await this.keycloak.getToken();
      this.keycloak.loadUserProfile().then(user => {
        console.log(user)
      }).catch(err => {
        console.log(err)
      })
    }
  }
}

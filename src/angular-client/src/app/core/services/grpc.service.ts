import {Injectable} from '@angular/core';
import {Client, CompatServiceDefinition, createChannel, createClientFactory, Metadata} from 'nice-grpc-web';
import {KeycloakService} from 'keycloak-angular';

@Injectable({
  providedIn: 'root'
})
export class GrpcService {
  channel = createChannel("http://127.0.0.1:5530");
  token?: string;
  clientFactory = createClientFactory().use((call, options) =>
    call.next(call.request, {
      ...options,
      metadata: Metadata(options.metadata).set(
        'Authorization',
        `Bearer ${this.token}`,
      ),
    }),
  );

  constructor(private readonly keycloak: KeycloakService) {
    this.keycloak.getToken().then(res =>{
      this.token = res;
    })
  }

  public getClient<Service extends CompatServiceDefinition>(definition: Service): Client<Service>{
    return this.clientFactory.create(definition, this.channel)
  }
}

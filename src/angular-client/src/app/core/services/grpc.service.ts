import {Injectable} from '@angular/core';
import {
  Client,
  ClientMiddlewareCall,
  CompatServiceDefinition,
  createChannel,
  createClientFactory,
  Metadata
} from 'nice-grpc-web';
import {KeycloakService} from 'keycloak-angular';
import {CallOptions, ClientMiddleware} from 'nice-grpc-common';

@Injectable({
  providedIn: 'root'
})
export class GrpcService {
  channel = createChannel("https://localhost:5531");
  clientFactory = createClientFactory().use(this.authMiddleware.bind(this));

  constructor(private readonly keycloak: KeycloakService) {
  }

  public getClient<Service extends CompatServiceDefinition>(definition: Service, middleware?: ClientMiddleware): Client<Service>{
    if(middleware)
      return this.clientFactory.use(middleware).create(definition, this.channel);
    return this.clientFactory.create(definition, this.channel)
  }

  async *authMiddleware<Request, Response>(
    call: ClientMiddlewareCall<Request, Response>,
    options: CallOptions,
  ){
    const token = await this.keycloak.getToken();
    return yield* call.next(call.request, {
      ...options,
      metadata: Metadata(options.metadata).set(
        'Authorization',
        `Bearer ${token}`,
      ),
    });
  }
}
export type Middleware = AsyncGenerator<Awaited<Response>, void | Awaited<Response>, undefined>;

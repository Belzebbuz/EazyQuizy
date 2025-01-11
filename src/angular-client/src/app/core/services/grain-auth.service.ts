import {inject, Injectable} from '@angular/core';
import {GrpcService} from './grpc.service';
import {AuthServiceClient, AuthServiceDefinition, IsAuthorizedRequest} from '../../../generated/auth/auth';
import {MetadataService} from './metadata.service';

@Injectable({
  providedIn: 'root'
})
export class GrainAuthService {
  client: AuthServiceClient;
  metadata = inject(MetadataService)
  constructor(private readonly grpc: GrpcService) {
    this.client = grpc.getClient(AuthServiceDefinition);
  }
  public async isAuthorized(id: string, action: string) {
    const request = IsAuthorizedRequest.create();
    request.grainId = id;
    request.action = action;
    return await this.client.isAuthorized(request);
  }
}

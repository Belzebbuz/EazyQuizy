import {inject, Injectable} from '@angular/core';
import {MetadataService} from './metadata.service';
import {GrpcService} from './grpc.service';
import {
  GameServiceClient,
  GameServiceDefinition,
  GetGameInfoRequest,
  PlayRequest,
  SetAnswerRequest
} from '../../../generated/quiz/game';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  client: GameServiceClient;
  metadata = inject(MetadataService);
  constructor(private readonly grpc: GrpcService) {
    this.client = grpc.getClient(GameServiceDefinition);
  }

  public async getInfo(id: string){
    const request = GetGameInfoRequest.create();
    request.gameId = id;
    return await this.client.getGameInfo(request);
  }
  public async play(id: string){
    const request = PlayRequest.create();
    request.gameId = id;
    return await this.client.play(request);
  }
  public async setAnswer(request: SetAnswerRequest){
    return await this.client.setAnswer(request);
  }
}

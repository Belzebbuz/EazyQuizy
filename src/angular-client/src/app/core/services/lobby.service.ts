import {inject, Injectable} from '@angular/core';
import {GrpcService} from './grpc.service';
import {
  ConnectPlayerRequest, CreateGameRequest,
  CreateLobbyRequest, DisconnectPlayerRequest, GetLobbyInfoRequest, InvitePlayerRequest,
  LobbyServiceClient,
  LobbyServiceDefinition,
  LobbySettings, RemovePlayerRequest, UpdateLobbySettingsRequest
} from '../../../generated/quiz/lobby';
import {MetadataService} from './metadata.service';

@Injectable({
  providedIn: 'root'
})
export class LobbyService {
  client: LobbyServiceClient;
  metadata = inject(MetadataService);
  constructor(private readonly grpc: GrpcService) {
    this.client = grpc.getClient(LobbyServiceDefinition);
  }
  public async create(quizId: string) {
    const request = CreateLobbyRequest.create();
    request.quizId = quizId;
    request.settings = LobbySettings.create();
    request.settings.isOpen = false;
    request.settings.maxPlayersCount = 30;
    request.settings.timePerQuestion = 120;
    return await this.client.create(request);
  }

  public async getInfo(lobbyId: string){
    const request = GetLobbyInfoRequest.create();
    request.id = lobbyId;
    return await this.client.getInfo(request);
  }
  public async invitePlayer(lobbyId: string, playerId: string){
    const request = InvitePlayerRequest.create();
    request.lobbyId = lobbyId;
    request.playerId = playerId;
    return await this.client.invitePlayer(request);
  }
  public async removePlayer(lobbyId: string, playerId: string){
    const request = RemovePlayerRequest.create();
    request.lobbyId = lobbyId;
    request.playerId = playerId;
    return await this.client.removePlayer(request);
  }
  public async updateSettings(lobbyId: string, settings: LobbySettings){
    const request = UpdateLobbySettingsRequest.create();
    request.lobbyId = lobbyId;
    request.settings = settings;
    return await this.client.updateSettings(request);
  }
  public async connect(lobbyId: string, playerName:string){
    const request = ConnectPlayerRequest.create();
    request.lobbyId = lobbyId;
    request.playerName = playerName;
    const result = await this.client.connectPlayer(request);
    this.metadata.storePlayerId(result.parameters['playerId'])
    return result;
  }
  public async disconnect(lobbyId: string){
    const request = DisconnectPlayerRequest.create();
    request.lobbyId = lobbyId;
    return await this.client.disconnectPlayer(request);
  }
  public async startGame(lobbyId: string) {
    const request = CreateGameRequest.create();
    request.lobbyId = lobbyId;
    return await this.client.startGame(request);
  }
}

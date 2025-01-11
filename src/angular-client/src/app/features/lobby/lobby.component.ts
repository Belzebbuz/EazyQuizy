import {Component, inject, OnDestroy, OnInit} from '@angular/core';
import {LobbyService} from '../../core/services/lobby.service';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import { Location } from '@angular/common';
import {NgForOf, NgIf} from '@angular/common';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {InputTextComponent} from '../../core/inputs/input-text/input-text.component';
import Keycloak from 'keycloak-js';
import {GetLobbyInfoResponse, LobbySettings} from '../../../generated/quiz/lobby';
import {PlusComponent} from '../../core/icons/plus/plus.component';
import {TrashComponent} from '../../core/icons/trash/trash.component';
import {NatsService} from '../../core/services/nats.service';
import {GrainStateChangedEvent} from '../../../generated/types/types';
import {InputNumberComponent} from '../../core/inputs/input-number/input-number.component';
import {GrainAuthService} from '../../core/services/grain-auth.service';
import {QRCodeComponent} from 'angularx-qrcode';
import {MetadataService} from '../../core/services/metadata.service';

@Component({
  selector: 'app-lobby',
  imports: [
    NgIf,
    ReactiveFormsModule,
    InputTextComponent,
    NgForOf,
    PlusComponent,
    TrashComponent,
    InputNumberComponent,
    QRCodeComponent,
    RouterLink
  ],
  templateUrl: './lobby.component.html',
})
export class LobbyComponent implements OnInit {
  service = inject(LobbyService);
  authService = inject(GrainAuthService);
  route = inject(ActivatedRoute);
  metadata = inject(MetadataService);
  router = inject(Router);
  keycloak = inject(Keycloak);
  nats = inject(NatsService);
  location = inject(Location);
  lobbyInfo?: GetLobbyInfoResponse
  lobbyId!: string;
  loggedIn = false;
  form = playerForm;
  settingsForm = lobbySettingsForm;
  serverError?: string;
  canUpdate = false;
  fullUrl = this.getFullUrl()
  isStartingGame = false;
  constructor() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.lobbyId = id;
  }
  getFullUrl(): string {
    const protocol = document.location.protocol;
    const host = document.location.host;
    const path = this.location.path();
    return `${protocol}//${host}${path}`;
  }
  get invitedPlayers() {
    return this.lobbyInfo?.players.filter(x => x.invited) ?? [];
  }

  get waitingPlayers() {
    return this.lobbyInfo?.players.filter(x => !x.invited) ?? [];
  }

  async ngOnInit() {
    try {
      await this.preloadForm();
      await this.initLobby();
    } catch {
      await this.router.navigate(['/'])
    }
  }
  private async initLobby() {
    this.lobbyInfo = await this.service.getInfo(this.lobbyId);
    const policyResult = await this.authService.isAuthorized(this.lobbyId, 'update');
    this.canUpdate = policyResult.authorized && !this.lobbyInfo.gameId;
    this.initSettingsForm();
    let playerId = this.metadata.getPlayerId();
    const playerIndex = this.lobbyInfo.players.findIndex(x => x.id == playerId)
    if (playerIndex > -1)
    {
      this.loggedIn = true;
      await this.subscribeToUpdates();
    }
  }
  async connect() {
    if (this.form.invalid) return;
    const name = this.form.controls.name.value!;
    this.metadata.storePlayerName(name);
    const result = await this.service.connect(this.lobbyId, name);
    this.serverError = result.message;
    if (result.succeeded) {
      this.loggedIn = true;
      this.lobbyInfo = await this.service.getInfo(this.lobbyId);
      await this.subscribeToUpdates();
    }
  }
  async copyUrlToClipboard(){
    await navigator.clipboard.writeText(this.fullUrl);
  }
  private async subscribeToUpdates() {
    if(!this.lobbyInfo) return
    for await (let _ of this.nats.subscribe<GrainStateChangedEvent>(
      this.lobbyInfo.updateChannel
    )) {
      this.lobbyInfo = await this.service.getInfo(this.lobbyId);
      this.initSettingsForm();
      const playerId = this.metadata.getPlayerId();
      const playerIndex = this.lobbyInfo.players.findIndex(x => x.id == playerId)
      if (playerIndex < 0) await this.router.navigate(['/']);
      if(this.lobbyInfo.gameId) await this.router.navigate(['game/' + this.lobbyInfo.gameId]);
    }
  }

  async addPlayer(playerId: string) {
    const result = await this.service.invitePlayer(this.lobbyId, playerId);
    this.serverError = result.message;
  }

  async removePlayer(playerId: string) {
    const result = await this.service.removePlayer(this.lobbyId, playerId);
    this.serverError = result.message;
  }

  private async preloadForm() {
    let name = this.metadata.getPlayerName();
    if (this.keycloak.authenticated) {
      const profile = await this.keycloak.loadUserProfile();
      if (profile.firstName)
        name ??= profile.firstName;
    }
    this.form.controls.name.setValue(name);
  }
  async submitSettings(){
    if(this.settingsForm.invalid) return;
    const request = LobbySettings.create();
    request.isOpen = this.settingsForm.controls.isOpen.value!;
    request.maxPlayersCount = this.settingsForm.controls.maxPlayersCount.value!;
    request.timePerQuestion = this.settingsForm.controls.timePerQuestion.value!;
    const result = await this.service.updateSettings(this.lobbyId, request);
    this.serverError = result.message;
  }

  async startGame(){
    this.isStartingGame = true;
    const result = await this.service.startGame(this.lobbyId);
    this.serverError = result.message;
    this.isStartingGame = false;
  }
  private initSettingsForm() {
    if(!this.lobbyInfo) return;
    this.settingsForm.controls.isOpen.setValue(this.lobbyInfo.settings?.isOpen ?? false);
    this.settingsForm.controls.maxPlayersCount.setValue(this.lobbyInfo.settings?.maxPlayersCount ?? 10);
    this.settingsForm.controls.timePerQuestion.setValue(this.lobbyInfo.settings?.timePerQuestion ?? 120);
  }
}

const playerForm = new FormGroup({
  name: new FormControl('', [Validators.required, Validators.maxLength(30)])
})
const lobbySettingsForm = new FormGroup({
  isOpen: new FormControl(false),
  maxPlayersCount: new FormControl(30, [Validators.min(1), Validators.max(100)]),
  timePerQuestion: new FormControl(120, [Validators.min(1), Validators.max(3600)])
})

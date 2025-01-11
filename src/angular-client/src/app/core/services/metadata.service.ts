import { Injectable } from '@angular/core';
import {Metadata} from 'nice-grpc-web';

@Injectable({
  providedIn: 'root'
})
export class MetadataService {
  playerIdKey = 'x-player-id';
  playerNameKey = 'x-player-name';
  constructor() { }
  getPlayerId() {
    return localStorage.getItem(this.playerIdKey);
  }
  getPlayerName() {
    return localStorage.getItem(this.playerNameKey);
  }
  storePlayerName(name: string){
    if(!name) return;
    localStorage.setItem(this.playerNameKey, name)
  }
  storePlayerId(id: string){
    if(!id) return;
    localStorage.setItem(this.playerIdKey, id)
  }
}

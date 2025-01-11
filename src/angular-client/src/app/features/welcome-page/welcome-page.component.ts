import {Component, inject, OnInit} from '@angular/core';
import {KeycloakService} from 'keycloak-angular';
import Keycloak from 'keycloak-js';
import {MotionService} from '../../core/services/motion.service';

@Component({
  selector: 'app-welcome-page',
  imports: [],
  templateUrl: './welcome-page.component.html',
})
export class WelcomePageComponent implements OnInit{
  keycloak = inject(Keycloak);
  motion = inject(MotionService);
  async login() {
    await this.keycloak.login();
  }
  async register(){
    await this.keycloak.register();
  }
  ngOnInit(): void {
    this.motion.isCirclesHidden.set(false)
  }
}

import {Component, OnInit} from '@angular/core';
import {KeycloakService} from 'keycloak-angular';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [],
  templateUrl: './navbar.component.html',
})
export class NavbarComponent implements OnInit {
  authText: string = 'Вход'
  loggedIn: boolean = false;
  helloText?: string;
  constructor(private keycloak: KeycloakService) {
  }

  async ngOnInit() {
    this.loggedIn = this.keycloak.isLoggedIn()
    this.authText = this.loggedIn ? 'Выйти' : 'Вход'
    if(this.loggedIn){
      const info = await this.keycloak.loadUserProfile();
      this.helloText = `Привет ${info.username}!`
    }
  }

  async login() {
    if (this.loggedIn) {
      await this.keycloak.logout()
    } else {
      await this.keycloak.login();
    }
  }

}
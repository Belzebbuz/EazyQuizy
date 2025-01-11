import {Component, inject, OnInit} from '@angular/core';
import {KeycloakEventType, KeycloakService} from 'keycloak-angular';
import Keycloak from 'keycloak-js';
import {ContentStaggerDirective} from '../../core/motions/directives/content-stagger.directive';
import {LogoutArrowComponent} from '../../core/icons/logout-arrow/logout-arrow.component';
import {MagnifyingGlassComponent} from '../../core/icons/magnifying-glass/magnifying-glass.component';
import {MoonStarsFillComponent} from '../../core/icons/moon-stars-fill/moon-stars-fill.component';
import {NgIf} from '@angular/common';
import {PlusComponent} from '../../core/icons/plus/plus.component';
import {RouterLink} from '@angular/router';
import {SunComponent} from '../../core/icons/sun/sun.component';
import {ThemeService} from '../../core/services/theme.service';
import {HomeComponent} from '../../core/icons/home/home.component';

@Component({
    selector: 'app-navbar',
  imports: [
    ContentStaggerDirective,
    LogoutArrowComponent,
    NgIf,
    PlusComponent,
    RouterLink,
    HomeComponent,
  ],
    templateUrl: './navbar.component.html'
})
export class NavbarComponent{
  keycloak = inject(Keycloak);
  theme = inject(ThemeService);
  loggedIn = this.keycloak.authenticated;
  async logout(){
    await this.keycloak.logout()
  }
}

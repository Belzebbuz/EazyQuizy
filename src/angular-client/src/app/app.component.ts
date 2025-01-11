import {Component, effect, inject} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {NavbarComponent} from './features/navbar/navbar.component';
import {NatsService} from './core/services/nats.service';
import Keycloak from 'keycloak-js';
import {ThemeService} from './core/services/theme.service';
import {MagneticBackgroundComponent} from './core/motions/magnetic-background/magnetic-background.component';
import {KEYCLOAK_EVENT_SIGNAL, KeycloakEventType} from 'keycloak-angular';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent,  MagneticBackgroundComponent, ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'Easy quiz';
  keycloak = inject(Keycloak);
  theme = inject(ThemeService);
  constructor(
    private readonly nats: NatsService
  ) {
    document.title = this.title;
    const keycloakSignal = inject(KEYCLOAK_EVENT_SIGNAL);
    effect(() => {
      const keycloakEvent = keycloakSignal();
      if (keycloakEvent.type === KeycloakEventType.TokenExpired)
        this.keycloak.updateToken().then(x => true);
    });
  }
}

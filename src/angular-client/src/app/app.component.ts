import {Component, inject, OnInit} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {GrpcService} from './core/services/grpc.service';
import {NavbarComponent} from './features/navbar/navbar.component';
import {FileUploadService} from './core/services/file-upload.service';
import {CreateQuizRequest, GetQuizInfoRequest, QuizServiceClient, QuizServiceDefinition} from '../generated/quiz/quiz';
import {NatsService} from './core/services/nats.service';
import {GrainStateChangedEvent} from '../generated/types/types';
import {KeycloakService} from 'keycloak-angular';
import {HttpClient, HttpResponse} from '@angular/common/http';
import {headers} from 'nats.ws';
import {DomSanitizer, SafeUrl} from '@angular/platform-browser';
import {ImageLoadService} from './core/services/image-load.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Easy quiz';
  client: QuizServiceClient;
  keycloak = inject(KeycloakService);
  imageUrl?: SafeUrl;
  imageService = inject(ImageLoadService);
  constructor(private readonly grpc: GrpcService,
              private readonly fileUpload: FileUploadService,
              private readonly nats: NatsService) {
    this.client = grpc.getClient(QuizServiceDefinition);
  }
  async listen(){
    const profile = await this.keycloak.loadUserProfile();
    for await (let data of this.nats.subscribe<GrainStateChangedEvent>("quiz.update." + profile.id)){
      console.log("Log from listen " + data.id)
    }
  }
  async create() {
    let rq = CreateQuizRequest.create()
    rq.name = "value";
    const resp = await this.client.create(rq);
    const req = GetQuizInfoRequest.create();
    req.id = resp.operationId;
    const getQuiz = await this.client.getInfo(req);
    console.log(getQuiz)
  }

  async onFileSelected(event: any) {
    let fileEvent = event as HTMLInputElement;
    if (!fileEvent || fileEvent.files == null)
      return;
    const file = fileEvent.files[0];
    const url = await this.fileUpload.uploadFile(file, 'quiz');
    if(!url)
      return;
    this.imageUrl = await this.imageService.getSafeUrl(url);
  }
}

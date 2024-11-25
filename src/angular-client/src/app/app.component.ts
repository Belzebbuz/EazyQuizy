import {Component, OnInit} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {GrpcService} from './core/services/grpc.service';
import {NavbarComponent} from './features/navbar/navbar.component';
import {FileUploadService} from './core/services/file-upload.service';
import {CreateQuizRequest, GetQuizInfoRequest, QuizServiceClient, QuizServiceDefinition} from '../generated/quiz/quiz';
import {NatsService} from './core/services/nats.service';
import {GrainStateChangedEvent} from '../generated/types/types';

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

  constructor(private readonly grpc: GrpcService,
              private readonly fileUpload: FileUploadService,
              private readonly nats: NatsService) {
    this.client = grpc.getClient(QuizServiceDefinition);
  }
  async listen(){
    for await (let data of this.nats.subscribe<GrainStateChangedEvent>("quiz.update")){
      console.log("Log from listen " + data.id)
    }
  }
  async create() {
    let rq = CreateQuizRequest.create()
    rq.name = "value";
    const resp = await this.client.create(rq);
    const req = GetQuizInfoRequest.create();
    req.id = resp.id;
    const getQuiz = await this.client.getInfo(req);
    console.log(getQuiz)
  }

  async onFileSelected(event: any) {
    let fileEvent = event as HTMLInputElement;
    if (!fileEvent || fileEvent.files == null)
      return;
    const file = fileEvent.files[0];
    await this.fileUpload.uploadFile(file, 'avatars');
  }
}

import {Component} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {
  CreateModuleRequest,
  ModuleServiceClient,
  ModuleServiceDefinition
} from '../generated/modules/module';
import {GrpcService} from './core/services/grpc.service';
import {NavbarComponent} from './features/navbar/navbar.component';
import {FileUploadService} from './core/services/file-upload.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent{
  title = 'Easy quiz';
  client: ModuleServiceClient;
  constructor(private readonly grpc: GrpcService, private readonly fileUpload: FileUploadService) {
    this.client = grpc.getClient(ModuleServiceDefinition)
  }
  async create(){
    let rq = CreateModuleRequest.create();
    rq.name = "value";
    const resp = await this.client.create(rq);
    console.log(resp);
  }

  async onFileSelected(event: any){
    let fileEvent = event as HTMLInputElement;
    if(!fileEvent || fileEvent.files == null)
      return;
    const file = fileEvent.files[0];
    await this.fileUpload.uploadFile(file, 'avatars');
  }
}

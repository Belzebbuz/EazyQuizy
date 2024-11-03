import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {createChannel, createClient} from 'nice-grpc-web';
import {CreateModuleRequest, ModuleServiceClient, ModuleServiceDefinition} from '../generated/modules/module';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'angular-client';
  channel = createChannel("http://127.0.0.1:5530");
  client: ModuleServiceClient = createClient(ModuleServiceDefinition, this.channel);
  async create(){
    let rq = CreateModuleRequest.create();
    rq.name = "value";
    const resp = await this.client.create(rq);
    console.log(resp);
  }
}

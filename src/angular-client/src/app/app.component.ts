import {Component} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {CreateModuleRequest, ModuleServiceClient, ModuleServiceDefinition} from '../generated/modules/module';
import {GrpcService} from './core/services/grpc.service';
import {NavbarComponent} from './features/navbar/navbar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent{
  title = 'angular-client';
  client: ModuleServiceClient;
  constructor(private readonly grpc: GrpcService) {
    this.client = grpc.getClient(ModuleServiceDefinition)
  }
  async create(){
    let rq = CreateModuleRequest.create();
    rq.name = "value";
    const resp = await this.client.create(rq);
    console.log(resp);
  }
}

import { Injectable } from '@angular/core';
import {DeepPartial} from '../../../generated/modules/module';
import {GrpcService} from './grpc.service';
import {Metadata} from 'nice-grpc-web';
import {FileChunk, FileServiceClient, FileServiceDefinition} from '../../../generated/files/file';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {

  client: FileServiceClient;
  constructor(private readonly grpc: GrpcService) {
    this.client = grpc.getClient(FileServiceDefinition)
  }
  async uploadFile(file: File, folder: string) : Promise<void> {
    const extension = file.name.split('.').pop();
    if(!extension)
      return ;
    const metadata = new Metadata();
    metadata.set('x-file-size', file.size.toString());
    metadata.set('x-folder', folder);
    metadata.set('x-extension', extension);

    const call = this.client.uploadFile(this.fileToChunks(file), {metadata});
    for await  (let report of call){
      console.log(report)
    }
  }
  async *fileToChunks(file: File, chunkSize: number = 1024 * 1024): AsyncIterable<DeepPartial<FileChunk>> {
    let offset = 0;
    const buffer = new ArrayBuffer(chunkSize);
    const uint8Array = new Uint8Array(buffer);
    while (offset < file.size) {
      const chunk = file.slice(offset, offset + chunkSize);
      const arrayBuffer = await this.readFileAsArrayBuffer(chunk);
      uint8Array.set(new Uint8Array(arrayBuffer));

      yield { chunkData: uint8Array.slice(0, arrayBuffer.byteLength) };

      offset += chunkSize;
    }
  }

  private readFileAsArrayBuffer(chunk: Blob): Promise<ArrayBuffer> {
    return new Promise((resolve, reject) => {
      const fileReader = new FileReader();
      fileReader.onload = () => resolve(fileReader.result as ArrayBuffer);
      fileReader.onerror = reject;
      fileReader.readAsArrayBuffer(chunk);
    });
  }
}

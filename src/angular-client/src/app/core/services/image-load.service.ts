import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {DomSanitizer, SafeUrl} from '@angular/platform-browser';
import {firstValueFrom} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ImageLoadService {

  httpClient = inject(HttpClient);
  sanitizer = inject(DomSanitizer);
  async getSafeUrl(imageUrl: string) : Promise<SafeUrl>{
    const source = this.httpClient.get(imageUrl, {responseType: 'blob'});
    const value = await firstValueFrom(source);
    const objectUrl = URL.createObjectURL(value);
    return this.sanitizer.bypassSecurityTrustUrl(objectUrl);
  }
}

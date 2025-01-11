import {Component, inject, Input, OnInit} from '@angular/core';
import {SafeUrl} from '@angular/platform-browser';
import {ImageLoadService} from '../../../core/services/image-load.service';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-game-image',
  imports: [
    NgIf
  ],
  templateUrl: './game-image.component.html',
})
export class GameImageComponent implements OnInit {
  @Input() questionImageUrl!: string;
  imageUrl?: SafeUrl;
  imageService = inject(ImageLoadService);

  async ngOnInit() {
    this.imageUrl = await this.imageService.getSafeUrl('/api/'+ this.questionImageUrl);
  }
}

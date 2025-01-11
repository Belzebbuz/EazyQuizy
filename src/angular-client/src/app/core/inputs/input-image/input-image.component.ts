import {AfterViewInit, Component, inject, Input, OnInit} from '@angular/core';
import {SafeUrl} from '@angular/platform-browser';
import {ImageLoadService} from '../../services/image-load.service';
import {FileUploadService} from '../../services/file-upload.service';
import {FormGroup} from '@angular/forms';
import {NgIf} from '@angular/common';
import {ArrowComponent} from '../../icons/arrow/arrow.component';

@Component({
  selector: 'app-input-image',
  imports: [
    NgIf,
    ArrowComponent
  ],
  templateUrl: './input-image.component.html',
})
export class InputImageComponent implements OnInit {
  imageUrl?: SafeUrl;
  imageService = inject(ImageLoadService);
  fileUpload = inject(FileUploadService);
  @Input() form!: FormGroup;
  @Input() controlName!: string;
  @Input() quizId?: string;

  ngOnInit(): void {
    this.form.get(this.controlName)?.valueChanges.subscribe(async (value) => {
      if(value)
        this.imageUrl = await this.imageService.getSafeUrl('/api/'+ value);
    });
  }
  async onFileSelected(event: any) {
    let fileEvent = event as HTMLInputElement;
    if (!fileEvent || fileEvent.files == null) return;
    const file = fileEvent.files[0];
    await this.uploadFile(file);
  }
  async uploadFile(file: File){
    const url = await this.fileUpload.uploadFile(file, this.quizId + '/');
    if (!url) return;
    this.form.get(this.controlName)?.setValue(url);
  }
  clearImage() {
    this.imageUrl = undefined;
    this.form.get(this.controlName)?.reset();
  }

  onPaste(event: ClipboardEvent): void {
    const items = event.clipboardData?.items;
    if (items) {
      for (const item of Array.from(items)) {
        if (item.type.startsWith('image/')) {
          const file = item.getAsFile();
          this.readImage(file);
          break;
        }
      }
    }
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      const file = files[0];
      if (file.type.startsWith('image/')) {
        this.readImage(file);
      }
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
  }

  private readImage(file: File | null) {
    if (file) {
      const reader = new FileReader();
      reader.onload = async () => {
        await this.uploadFile(file);
      };
      reader.readAsDataURL(file);
    }
  }
}

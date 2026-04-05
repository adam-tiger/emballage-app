import { ChangeDetectionStrategy, Component, inject, input, output, signal } from '@angular/core';
import { NgIf } from '@angular/common';
import { finalize } from 'rxjs';
import { FileUploadModule, FileSelectEvent } from 'primeng/fileupload';
import { ButtonModule } from 'primeng/button';
import { MessageService } from 'primeng/api';
import { AdminProductService } from '../../services/admin-product.service';
import { ApiError } from '../../../../../shared/models/api-error.model';
import { UploadProductImageResponse } from '../../models/add-price-tier.request';

@Component({
  selector: 'app-product-image-uploader',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [NgIf, FileUploadModule, ButtonModule],
  template: `
    <div class="uploader">

      @if (currentImageUrl()) {
        <div class="uploader__current">
          <img [src]="currentImageUrl()!"
               alt="Image actuelle"
               class="uploader__preview" />
          <span class="uploader__label">Image actuelle</span>
        </div>
      }

      <p-fileUpload
        mode="basic"
        accept="image/jpeg,image/png,image/webp"
        [maxFileSize]="5242880"
        chooseLabel="Choisir une image"
        [auto]="false"
        (onSelect)="onFileSelect($event)" />

      @if (selectedFile()) {
        <div class="uploader__actions">
          <span class="uploader__filename">{{ selectedFile()!.name }}</span>
          <p-button
            label="Uploader"
            icon="pi pi-upload"
            [loading]="isUploading()"
            (onClick)="upload()" />
        </div>
      }

    </div>
  `,
  styleUrl: './product-image-uploader.component.scss'
})
export class ProductImageUploaderComponent {
  private readonly adminService    = inject(AdminProductService);
  private readonly messageService  = inject(MessageService);

  productId       = input.required<string>();
  currentImageUrl = input<string | null>(null);

  imageUploaded = output<UploadProductImageResponse>();

  readonly isUploading  = signal(false);
  readonly selectedFile = signal<File | null>(null);

  onFileSelect(event: FileSelectEvent): void {
    const file = event.files[0];
    if (file) {
      this.selectedFile.set(file);
    }
  }

  upload(): void {
    const file = this.selectedFile();
    if (!file) return;

    this.isUploading.set(true);
    this.adminService
      .uploadImage(this.productId(), file, true)
      .pipe(finalize(() => this.isUploading.set(false)))
      .subscribe({
        next: res => {
          this.imageUploaded.emit(res);
          this.selectedFile.set(null);
        },
        error: (err: ApiError) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Erreur upload',
            detail: err.message
          });
        }
      });
  }
}

import { CommonModule } from '@angular/common';
import {
  Component, ElementRef, EventEmitter, Input, NgModule, OnInit,
  Output,
} from '@angular/core';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { DxFileUploaderModule } from 'devextreme-angular/ui/file-uploader';
import notify from 'devextreme/ui/notify';

@Component({
  selector: 'form-photo',
  templateUrl: './form-photo.component.html',
  styleUrls: ['./form-photo.component.scss'],
})
export class FormPhotoComponent implements OnInit {
  @Input() link!: string;
  @Output() linkChange = new EventEmitter<string>(); // el valor saliente (cuando cambia)

  @Input() editable = false;

  @Input() size = 124;

  @Input() uploadUrl: string = "https://localhost:7062/api/Upload/image/users";

  imageUrl: SafeStyle;


  hostRef = this.elRef.nativeElement;


  constructor(private elRef:ElementRef, private sanitizer: DomSanitizer) {}

  ngOnInit() {
    if (this.link) { // verifica que no sea null, undefined ni ''
      this.imageUrl = this.sanitizer.bypassSecurityTrustStyle(`url('${this.link}')`);
    } else {
      // Opcional: define un valor por defecto o maneja el caso donde no hay imagen
      this.imageUrl = ''; // o podrías poner una imagen por defecto
    }
  }
  onUploadStarted(e: any) {
    console.log('🚀 Upload started:', e);
  }

  onUploaded(e: any): void {
    const response = e.request.response;
    //verificr si la respuesta es correcta
    if (e.request.status === 200) {
      this.link = response.body.imageUrl;
      this.imageUrl = this.sanitizer.bypassSecurityTrustStyle(`url('${this.link}')`);
      this.updateLink(this.link); // emite el nuevo valor
    }
    else {
      // Manejar el error de carga aquí
      console.error('Error uploading file:', e.request.statusText);
      notify('Error uploading file', 'error', 4000);
    }
  }

  updateLink(newUrl: string) {
    this.link = newUrl;
    this.linkChange.emit(newUrl);
  }
}

@NgModule({
  imports: [
    DxFileUploaderModule,
    CommonModule
  ],
  declarations: [FormPhotoComponent],
  exports: [FormPhotoComponent],
})
export class FormPhotoModule { }

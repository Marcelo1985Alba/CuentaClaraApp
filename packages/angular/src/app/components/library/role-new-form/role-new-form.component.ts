import { Component, NgModule, ViewChild } from '@angular/core';
import { DxFormModule } from 'devextreme-angular/ui/form';
import { DxTextBoxModule } from 'devextreme-angular/ui/text-box';
import { DxValidatorModule } from 'devextreme-angular/ui/validator';
import { FormTextboxModule } from '../../utils/form-textbox/form-textbox.component';
import { FormPhotoUploaderModule } from '../../utils/form-photo-uploader/form-photo-uploader.component';
import { CommonModule } from '@angular/common';
import { getSizeQualifier } from 'src/app/services/screen.service';
import { IRoleDTO } from 'src/app/core/models/role/role-dto';

@Component({
  selector: 'role-new-form',
  templateUrl: './role-new-form.component.html'
})
export class RoleNewFormComponent {

  getSizeQualifier = getSizeQualifier;
  newRole: IRoleDTO = {
    id: '',
    name: '',
    description: ''
  };

  getNewRoleData = ()=> ({ ...this.newRole })
}


@NgModule({
  imports: [
    DxTextBoxModule,
    DxFormModule,
    DxValidatorModule,

    FormTextboxModule,
    FormPhotoUploaderModule,

    CommonModule,
  ],
  declarations: [RoleNewFormComponent],
  exports: [RoleNewFormComponent],
})
export class RoleNewFormModule { }

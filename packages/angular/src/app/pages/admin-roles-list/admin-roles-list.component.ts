
import { Component, NgModule, OnInit, ViewChild } from '@angular/core';
import { error } from 'console';
import notify from 'devextreme/ui/notify';
import { DxToolbarModule } from 'devextreme-angular/ui/toolbar';
import { DxDataGridComponent, DxDataGridModule, DxDataGridTypes } from 'devextreme-angular/ui/data-grid';
import { DxButtonModule } from 'devextreme-angular/ui/button';
import { DxTabsModule } from 'devextreme-angular/ui/tabs';
import jsPDF from 'jspdf';
import { Workbook } from 'exceljs';
import { saveAs } from 'file-saver-es';
import { exportDataGrid as exportDataGridToPdf } from 'devextreme/pdf_exporter';
import { exportDataGrid as exportDataGridToXLSX } from 'devextreme/excel_exporter';
import { IRoleDTO } from 'src/app/core/models/role/role-dto';
import { RolesService } from './roles.service';
import DataSource from 'devextreme/data/data_source';
import { FormPopupModule } from 'src/app/components';
import { IApiResponseData } from 'src/app/core/models/response/response';
import { Router } from '@angular/router';
import { RoleNewFormComponent, RoleNewFormModule } from 'src/app/components/library/role-new-form/role-new-form.component';

@Component({
  selector: 'app-admin-roles-list',
  templateUrl: './admin-roles-list.component.html',
  styleUrl: './admin-roles-list.component.scss'
})
export class AdminRolesListComponent implements OnInit {
  @ViewChild(DxDataGridComponent, { static: true }) dataGrid: DxDataGridComponent;
  @ViewChild(RoleNewFormComponent, { static: false }) roleNewForm: RoleNewFormComponent;
  rolesList: IRoleDTO[] = [];
  roles = new DataSource<IRoleDTO[], string>({
        key: 'id',
        load: () => new Promise((resolve, reject) => {
          this.roleService.getRoles().subscribe({
              next: (response: IApiResponseData<IRoleDTO[]>) => {
                this.rolesList = response.data;
                resolve(this.rolesList);
              },
              error: ({message}) =>{
                reject(message)
                this.router.navigate(['/auth/login']);
              }
            })
        }),
      });
  userId: string;
  isPanelOpened = false;
  isAddRolePopupOpened = false;
  constructor(private roleService: RolesService, private router: Router) { }

  ngOnInit(): void {
    this.getRoles();
  }

  getRoles() {
    this.roles = new DataSource<IRoleDTO[], string>({
      key: 'id',
      load: () => new Promise((resolve, reject) => {
        this.roleService.getRoles().subscribe({
            next: (response: IApiResponseData<IRoleDTO[]>) => {
              this.rolesList = response.data;
              resolve(this.rolesList);
            },
            error: ({message}) =>{
              notify({message}, 'error', 4000);
              reject(message)
              this.router.navigate(['/auth/login']);
            }
          })
      }),
    });
  }

  refreshData = ()=>{
    this.getRoles();
  }

  rowClick(e: DxDataGridTypes.RowClickEvent) {
    const { data } = e;

    this.userId = data.id;
    this.isPanelOpened = true;
  }

  onExporting(e) {
      if (e.format === 'pdf') {
        const doc = new jsPDF();
        exportDataGridToPdf({
          jsPDFDocument: doc,
          component: e.component,
        }).then(() => {
          doc.save('Users.pdf');
        });
      } else {
        const workbook = new Workbook();
        const worksheet = workbook.addWorksheet('Users');

        exportDataGridToXLSX({
          component: e.component,
          worksheet,
          autoFilterEnabled: true,
        }).then(() => {
          workbook.xlsx.writeBuffer().then((buffer) => {
            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Users.xlsx');
          });
        });
        e.cancel = true;
      }
  }

  addRole() {
    this.isAddRolePopupOpened = true;
  }

  onClickSaveNewRole = () => {
    const { name, description} = this.roleNewForm.getNewRoleData();
    const newRole: IRoleDTO = { id: '', name, description };
    this.roleService.createRole(newRole).subscribe({
      next: (respose) => {
        this.isAddRolePopupOpened = false;
        notify({
          message: `New role "${name} ${description}" saved`,
          position: { at: 'bottom center', my: 'bottom center' }
        },
        'success');
        this.rolesList.push(respose);
        this.roles.reload();
        }
      ,
      error: ({message}) => {
        notify({message}, 'error', 4000);
      }
    });

  };
}

@NgModule({
  imports: [
    DxToolbarModule,
    DxDataGridModule,
    DxTabsModule,
    DxButtonModule,
    FormPopupModule,
    RoleNewFormModule
],
  providers: [],
  exports: [],
  declarations: [AdminRolesListComponent],
})
export class AdminRolesListModule {}{
}

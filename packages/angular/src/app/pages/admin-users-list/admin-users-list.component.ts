import { Component, NgModule, OnInit, ViewChild } from '@angular/core';
import {
  DxButtonModule,
  DxDataGridModule,
  DxDataGridComponent,
  DxDropDownButtonModule,
  DxSelectBoxModule,
  DxTextBoxModule,
  DxToolbarModule,
  DxTabsModule,
} from 'devextreme-angular';
import { DxDataGridTypes } from 'devextreme-angular/ui/data-grid';
import { exportDataGrid as exportDataGridToPdf } from 'devextreme/pdf_exporter';
import { exportDataGrid as exportDataGridToXLSX } from 'devextreme/excel_exporter';
import { UsersService } from './users.service';
import DataSource from 'devextreme/data/data_source';
import { IUserDto } from 'src/app/core/models/user/user-dto';
import { IApiResponseData } from 'src/app/core/models/response/response';
import notify from 'devextreme/ui/notify';
import jsPDF from 'jspdf';
import { Workbook } from 'exceljs';
import { saveAs } from 'file-saver-es';
import { Router } from '@angular/router';
import { UserPanelModule } from 'src/app/components';
import { UserPanelEditModule } from 'src/app/components/library/user-panel-edit/user-panel-edit.component';

@Component({
  selector: 'app-admin-users',
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss'
})
export class AdminUsersListComponent implements OnInit {

  @ViewChild(DxDataGridComponent, { static: true }) dataGrid: DxDataGridComponent;
  refresh: any;
  displayGrid: any;
  chooseColumnDataGrid: any;
  exportDataGridToXSLX: any;
  searchDataGrid: any;
  userId: string;
  isPanelOpened = false;
  user: IUserDto;
  dataSource = new DataSource<IUserDto[], string>({
      key: 'id',
      load: () => new Promise((resolve, reject) => {
        this.usersService.getUsers().subscribe({
            next: (data: IApiResponseData<IUserDto[]>) => resolve(data),
            error: ({message}) =>{
              reject(message)
              this.router.navigate(['/auth/login']);
            }
          })
      }),
    });

  datasource: IUserDto[];
  constructor(private usersService : UsersService, private router: Router ){

  }

  ngOnInit(): void {
      this.refreshData();
    }

    refreshData= ()=>{

      this.dataSource = new DataSource<IUserDto[], string>({
        key: 'id',
        load: () => new Promise((resolve, reject) => {
          this.usersService.getUsers().subscribe({
              next: (data: IApiResponseData<IUserDto[]>) => resolve(data),
              error: ({message}) =>{
                reject(message)
                this.router.navigate(['/auth/login']);
              }
            })
        }),
      });
    }

    rowClick(e: DxDataGridTypes.RowClickEvent) {
      this.user = e.data;

      this.userId = this.user.id;
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

  addUser() {
    throw new Error('Method not implemented.');
  }

  onOpenedChange = (value: boolean) => {
    if (!value) {
      this.userId = null;
    }
  };

  onPinnedChange = () => {
    this.dataGrid.instance.updateDimensions();
  };
}


@NgModule({
  imports:[
    DxToolbarModule,
    DxDataGridModule,
    DxTabsModule,
    DxButtonModule,
    UserPanelEditModule
],
  providers: [],
  exports: [],
  declarations: [AdminUsersListComponent],
})
export class AdminUsersListModule {}{
}


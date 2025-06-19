import { IUpdateUserDto } from './../../../core/models/user/user-dto';
import { DxScrollViewModule } from 'devextreme-angular/ui/scroll-view';
import { AfterViewChecked, Component, EventEmitter, Input, NgModule, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { DxFormModule } from 'devextreme-angular/ui/form';
import { DxTextBoxModule } from 'devextreme-angular/ui/text-box';
import { DxValidatorModule } from 'devextreme-angular/ui/validator';
import { distinctUntilChanged, Subject, Subscription } from 'rxjs';
import { IUserDto, IUserDtoDetails } from 'src/app/core/models/user/user-dto';
import { FormTextboxModule } from '../../utils/form-textbox/form-textbox.component';
import { FormPhotoUploaderModule } from '../../utils/form-photo-uploader/form-photo-uploader.component';
import { CommonModule } from '@angular/common';
import { DxAccordionModule, DxButtonModule, DxLoadPanelModule, DxToolbarModule, DxValidationGroupModule } from 'devextreme-angular';
import { ScreenService } from 'src/app/services/screen.service';
import { Router } from '@angular/router';
import { DxButtonTypes } from 'devextreme-angular/ui/button';
import { UsersService } from 'src/app/pages/admin-users-list/users.service';
import { FormPhotoModule } from '../../utils/form-photo/form-photo.component';
import notify from 'devextreme/ui/notify';

@Component({
  selector: 'user-panel-edit',
  templateUrl: './user-panel-edit.component.html',
  styleUrl: './user-panel-edit.component.scss'
})
export class UserPanelEditComponent implements OnInit, OnChanges, AfterViewChecked, OnDestroy {
    @Input() isOpened = false;

    // @Input() user: IUserDto = {
    //   id: '',
    //   userName: '',
    //   firstName: '',
    //   lastName: '',
    //   email: '',
    //   phoneNumber: '',
    //   imageUrl: ''
    // };

    @Input() userId: string = '';


    @Output() isOpenedChange = new EventEmitter<boolean>();

    @Output() pinnedChange = new EventEmitter<boolean>();

    private pinEventSubject = new Subject<boolean>();

    formData : IUserDtoDetails = {
      id: '',
      userName: '',
      firstName: '',
      lastName: '',
      email: '',
      phoneNumber: '',
      imageUrl: '',
      roles: [],
    };

    contactData: IUserDtoDetails = {
      id: '',
      userName: '',
      firstName: '',
      lastName: '',
      email: '',
      phoneNumber: '',
      imageUrl: '',
      roles: []
    };

    pinned = false;

    isLoading = true;

    isEditing = false;

    isPinEnabled = false;

    userPanelSubscriptions: Subscription[] = [];

    constructor(private screen: ScreenService,  private router: Router, private userService: UsersService) {
        this.userPanelSubscriptions.push(
          this.screen.changed.subscribe(this.calculatePin),
          this
            .pinEventSubject
            .pipe(distinctUntilChanged())
            .subscribe(this.pinnedChange)
        );
      }

      ngOnInit(): void {
        this.calculatePin();
      }

      ngAfterViewChecked(): void {
        this.pinEventSubject.next(this.pinned);
      }

      ngOnChanges(changes: SimpleChanges): void {
        const { userId } = changes;

        if (userId?.currentValue) {
          this.loadUserById(userId.currentValue);
        }
      }

      ngOnDestroy(): void {
        this.userPanelSubscriptions.forEach((sub) => sub.unsubscribe());
      }

      loadUserById = (id: string) => {
        this.isLoading = true;

        this.userService.getUserById(id).subscribe((data) => {
          this.formData = data.data;
          this.contactData = { ...this.formData };
          this.isLoading = false;
          this.isEditing = false;
        })
      };

      onClosePanel = () => {
        this.isOpened = false;
        this.pinned = false;
        this.isOpenedChange.emit(this.isOpened);
      };

      onSaveClick = ({ validationGroup } : DxButtonTypes.ClickEvent) => {
        if (!validationGroup.validate().isValid) return;

        this.contactData = { ...this.formData };

        const updateUser: IUpdateUserDto = this.userService.pickUpdateUserFields(this.contactData);

        this.userService.updateUserDetails(this.contactData.id, updateUser).subscribe({
          next: (response) => {
            /* this.formData = response.data;
            this.contactData = { ...this.formData }; */
            this.isEditing = !this.isEditing;
            notify('User updated successfully', 'success', 2000);
          },
          error: (error) => {
            this.isEditing = !this.isEditing;
            console.error(error);
            notify('Error updating user', 'error', 2000);
          }
        });
      };

      onPinClick = () => {
        this.pinned = !this.pinned;
      };

      calculatePin = () => {
        this.isPinEnabled = this.screen.sizes['screen-large'] || this.screen.sizes['screen-medium'];
        if (this.pinned && !this.isPinEnabled) {
          this.pinned = false;
        }
      };

      toggleEdit = () => {
        this.isEditing = !this.isEditing;
      };

      cancelHandler() {
        this.toggleEdit();
        this.formData = { ...this.contactData };
      }

      navigateToDetails = () => {
        this.router.navigate(['/crm-contact-details']);
      };
}

@NgModule({
  imports: [
    DxToolbarModule,
    DxButtonModule,
    DxTextBoxModule,
    DxScrollViewModule,
    DxValidatorModule,
    DxValidationGroupModule,
    DxButtonModule,
    DxLoadPanelModule,
    DxAccordionModule,
    DxFormModule,
    DxValidatorModule,
    FormTextboxModule,
    FormPhotoModule,
    FormPhotoUploaderModule,
    CommonModule,
  ],
  declarations: [UserPanelEditComponent],
  exports: [UserPanelEditComponent],
})
export class UserPanelEditModule { }

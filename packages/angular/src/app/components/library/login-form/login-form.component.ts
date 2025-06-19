import { CommonModule } from '@angular/common';
import { Component, NgModule, Input, OnInit, ViewChild } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

import { LoginOauthModule } from 'src/app/components/library/login-oauth/login-oauth.component';
import { DxFormModule } from 'devextreme-angular/ui/form';
import { DxLoadIndicatorModule } from 'devextreme-angular/ui/load-indicator';
import { DxButtonModule, DxButtonTypes } from 'devextreme-angular/ui/button';
import notify from 'devextreme/ui/notify';
import { IResponse, ThemeService } from 'src/app/services';
import { error } from 'console';
import { LoginService } from 'src/app/shared/services/login.service';
import { IApiResponseData } from 'src/app/core/models/response/response';
import { IUserDto } from 'src/app/core/models/user/user-dto';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.scss'],
})
export class LoginFormComponent implements OnInit {
  @Input() resetLink = '/auth/reset-password';
  @Input() createAccountLink = '/auth/create-account';

  @ViewChild('formComponent', { static: false }) formComponent: any; // Referencia al dx-form

  defaultAuthData: IApiResponseData<IUserDto>;

  btnStylingMode: DxButtonTypes.ButtonStyle;

  passwordMode = 'password';

  loading = false;

  formData: any = {};

  passwordEditorOptions = {
    placeholder: 'Password',
    stylingMode:'filled',
    mode: this.passwordMode,
    value: 'password',
    buttons: [{
      name: 'password',
      location: 'after',
      options: {
        icon: 'info',
        stylingMode:'text',
        onClick: () => this.changePasswordMode(),
      }
    }]
  }


  constructor( private loginService: LoginService,
    private router: Router,
    private themeService: ThemeService) {
    this.themeService.isDark.subscribe((value: boolean) => {
      this.btnStylingMode = value ? 'outlined' : 'contained';
    });
  }

  changePasswordMode() {
    this.passwordMode = this.passwordMode === 'text' ? 'password' : 'text';


    if (this.formComponent && this.formComponent.instance) {
      const passwordEditor = this.formComponent.instance.getEditor('password');
      if (passwordEditor) {
        passwordEditor.option('mode', this.passwordMode);
        console.log('Password mode changed to:', this.passwordMode);
      } else {
        console.error('Password editor not found');
      }
    }

  };

  async onSubmit(e: Event) {
    e.preventDefault();
    const { userName, password } = this.formData;
    this.loading = true;

    this.loginService.logIn(userName, password)
      .then((userLogin)=> {

        console.log('Login response:', userLogin); // LOG 1
        console.log('userLogin.success:', userLogin.success); // LOG 2
        console.log('Type of success:', typeof userLogin.success);

        // Si el login es exitoso, obtenemos los datos del usuario
        if(userLogin.success){
          console.log('SUCCESS: Navigating to home'); // LOG 4
          this.loading = false;
          this.router.navigate(['/']);
          // this.loginService.getPerfil().subscribe({
          //   next: (userData) => {
          //     console.log(userData);
          //     // Aquí puedes manejar los datos del usuario

          //     // Redirigir al dashboard u otra página
          //     this.router.navigate(['/']);
          //   },
          //   error: (err) => {
          //     console.error('Error al obtener los datos del usuario:', err);
          //     this.loading = false;
          //     notify('Error al obtener los datos del usuario', 'error', 4500);
          //   }
          // });
        }
        else {
          console.log('FAILED: Showing error message'); // LOG 5
          // Manejar error de credenciales
          const errorMessage = userLogin.message || 'Error en credenciales';
          notify(errorMessage, 'error', 4500);
        }
      })
      .catch(responseError => {
        this.loading = false;
        notify('Error en credenciales', 'error', 4500)
      });

  }

  onCreateAccountClick = () => {
    this.router.navigate([this.createAccountLink]);
  };

  async ngOnInit(): Promise<void> {
    // this.defaultAuthData = await this.authService.getUser();
    this.passwordEditorOptions = {
      placeholder: 'Password',
      stylingMode:'filled',
      mode: this.passwordMode,
      value: 'password',
      buttons: [{
        name: 'password',
        location: 'after',
        options: {
          icon: 'info',
          stylingMode:'text',
          onClick: () => this.changePasswordMode(),
        }
      }]
    }
    this.defaultAuthData = null;
  }
}
@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    LoginOauthModule,
    DxFormModule,
    DxLoadIndicatorModule,
    DxButtonModule
  ],
  declarations: [LoginFormComponent],
  exports: [LoginFormComponent],
})
export class LoginFormModule { }

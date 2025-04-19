import { CommonModule } from '@angular/common';
import { Component, NgModule, Input, OnInit } from '@angular/core';
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
  };

  async onSubmit(e: Event) {
    e.preventDefault();
    const { userName, password } = this.formData;
    this.loading = true;

    this.loginService.logIn(userName, password)
      .then((userLogin)=> {
        // Si el login es exitoso, obtenemos los datos del usuario
        if(userLogin.success){
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

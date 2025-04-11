import { Injectable } from '@angular/core';
import { resolve } from 'path';
import { IApiResponseData } from 'src/app/core/models/response/response';
import { IUserDto } from 'src/app/core/models/user/user-dto';
import { AuthService } from 'src/app/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(private authService: AuthService) { }



  logIn(userName: string, password: string, callback?:any) : Promise<IApiResponseData<IUserDto>>{

    const cb = callback || function(){};
    return new Promise((resolve, reject) =>{
      this.authService.logIn(userName, password).subscribe({
        next: res=> {

          resolve(res);

          return cb();
      },
      error: (err) => {
        console.error(err.error);
        reject(err);
        return cb(err);
      }});
    });
  }

  getPerfil(){
    return this.authService.getUser();
  }
}



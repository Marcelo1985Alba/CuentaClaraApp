import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IUserDto } from 'src/app/core/models/user/user-dto';
import { IApiResponseData } from 'src/app/core/models/response/response';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private http: HttpClient) { }

  getUsers(): Observable<IApiResponseData<IUserDto[]> >{
      return this.http.get<IApiResponseData<IUserDto[]>>('https://localhost:7062/api/Users');
  }
}

import { IUserDtoDetails } from './../../core/models/user/user-dto';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IUpdateUserDto, IUserDto } from 'src/app/core/models/user/user-dto';
import { IApiResponseData } from 'src/app/core/models/response/response';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private http: HttpClient) { }

  getUsers(): Observable<IApiResponseData<IUserDto[]> >{
      return this.http.get<IApiResponseData<IUserDto[]>>('https://localhost:7062/api/Users');
  }

  getUserById(id:string): Observable<IApiResponseData<IUserDtoDetails> >{
    //agregar backtip para agregar id como parametro ala url
    return this.http.get<IApiResponseData<IUserDtoDetails>>(`https://localhost:7062/api/Users/${id}`);
  }

  updateUserDetails(id:string, userDetails: IUpdateUserDto): Observable<IApiResponseData<IUserDtoDetails> >{
    //agregar backtip para agregar id como parametro ala url
    return this.http.put<IApiResponseData<IUserDtoDetails>>(`https://localhost:7062/api/Users/${id}`, userDetails);
  }

  pickUpdateUserFields(data : IUserDtoDetails): IUpdateUserDto {
    return {
      firstName: data.firstName,
      lastName: data.lastName,
      email: data.email,
      phoneNumber: data.phoneNumber,
      roles: data.roles,
      imageUrl: data.imageUrl
      // agrega solo las propiedades de IUpdateUserDto
    };
  }
}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IApiResponseData } from 'src/app/core/models/response/response';
import { IRoleDTO } from 'src/app/core/models/role/role-dto';

@Injectable({
  providedIn: 'root'
})
export class RolesService {
  apiUrl: 'https://localhost:7062/api/Roles';

  constructor(private http: HttpClient) { }

  getRoles(){
    return this.http.get<IApiResponseData<IRoleDTO[]>>('https://localhost:7062/api/Roles');
  }

  createRole(role: IRoleDTO) {
    return this.http.post<IRoleDTO>('https://localhost:7062/api/Roles', role);
  }
}

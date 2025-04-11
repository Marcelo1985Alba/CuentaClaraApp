export interface IUserDto {
  id: string;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  imageUrl: string;
  // token: string;
}

export interface ICreateUserDto {
  userName: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
}

export interface IUpdateUserDto {
  firstName: string;
  lastName: string;
  phoneNumber: string;
}

export interface ILoginDto {
  userName: string;
  password: string;
}


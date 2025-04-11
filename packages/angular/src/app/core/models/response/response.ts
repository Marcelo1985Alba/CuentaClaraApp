export interface IApiResponse {
  success: boolean;
  message?: string;
  errors?: string[];
}

export interface IApiResponseData<T> extends IApiResponse {
  data?: T;
}

export type ResponseStatus = 'success' | 'failure' | 'warning';

export interface ApiResponse<T = any> {
  responseStatus: ResponseStatus;
  responseMessage: string;
  responseObject: T;
}

export interface TokenResponse {
  token: string;
  username?: string;
  email?: string;
}

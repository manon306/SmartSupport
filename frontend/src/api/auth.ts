import apiClient from './axios';
import type { AuthResponse, LoginRequest, RegisterRequest, RefreshRequest, LogoutRequest } from '../types';

export const authApi = {
  login: async (data: LoginRequest) => {
    const response = await apiClient.post<AuthResponse>('/Auth/login', data);
    return response.data;
  },
  
  register: async (data: RegisterRequest) => {
    const response = await apiClient.post<AuthResponse>('/Auth/register', data);
    return response.data;
  },
  
  refreshToken: async (data: RefreshRequest) => {
    const response = await apiClient.post<AuthResponse>('/Auth/refresh-token', data);
    return response.data;
  },
  
  logout: async (data: LogoutRequest) => {
    const response = await apiClient.post('/Auth/logout', data);
    return response.data;
  }
};

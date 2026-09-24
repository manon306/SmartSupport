import apiClient from './axios';
import type { DashboardStats, User, ChangeRoleRequest } from '../types';

export const adminApi = {
  getDashboardStats: async () => {
    const response = await apiClient.get<DashboardStats>('/Admin/dashboard');
    return response.data;
  },
  
  getUsers: async () => {
    const response = await apiClient.get<User[]>('/Admin/users');
    return response.data;
  },
  
  getUserById: async (userId: string) => {
    const response = await apiClient.get<User>(`/Admin/users/${userId}`);
    return response.data;
  },
  
  changeUserRole: async (userId: string, data: ChangeRoleRequest) => {
    const response = await apiClient.put<User>(`/Admin/users/${userId}/role`, data);
    return response.data;
  },
  
  getAgents: async () => {
    const response = await apiClient.get<User[]>('/Admin/agents');
    return response.data;
  }
};

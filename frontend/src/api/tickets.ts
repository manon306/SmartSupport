import apiClient from './axios';
import type { Ticket, CreateTicketRequest, UpdateTicketRequest, TicketFilter, PagedResult } from '../types';
import { TicketStatus } from '../types';

export const ticketsApi = {
  getTickets: async (filter: TicketFilter) => {
    const response = await apiClient.get<PagedResult<Ticket>>('/Ticket', { params: filter });
    return response.data;
  },
  
  getTicketById: async (id: number) => {
    const response = await apiClient.get<Ticket>(`/Ticket/${id}`);
    return response.data;
  },
  
  createTicket: async (data: CreateTicketRequest) => {
    const response = await apiClient.post<Ticket>('/Ticket', data);
    return response.data;
  },
  
  updateTicket: async (id: number, data: UpdateTicketRequest) => {
    const response = await apiClient.put<Ticket>(`/Ticket/${id}`, data);
    return response.data;
  },
  
  deleteTicket: async (id: number) => {
    const response = await apiClient.delete(`/Ticket/${id}`);
    return response.data;
  },
  
  assignTicket: async (ticketId: number, agentId: string) => {
    const response = await apiClient.put<Ticket>(`/Ticket/${ticketId}/assign/${agentId}`);
    return response.data;
  },
  
  changeStatus: async (ticketId: number, status: TicketStatus) => {
    const response = await apiClient.put<Ticket>(`/Ticket/${ticketId}/status/${status}`);
    return response.data;
  }
};

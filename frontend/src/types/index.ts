// Matches Domain/Enums/TicketStatus.cs
export const TicketStatus = {
  Open: 0,
  InProgress: 1,
  Resolved: 2,
  Closed: 3,
} as const;
export type TicketStatus = typeof TicketStatus[keyof typeof TicketStatus];

// Matches Domain/Enums/TicketPriority.cs
export const TicketPriority = {
  Low: 0,
  Medium: 1,
  High: 2,
  Critical: 3,
} as const;
export type TicketPriority = typeof TicketPriority[keyof typeof TicketPriority];

export const TicketStatusLabels: Record<TicketStatus, string> = {
  [TicketStatus.Open]: 'Open',
  [TicketStatus.InProgress]: 'In Progress',
  [TicketStatus.Resolved]: 'Resolved',
  [TicketStatus.Closed]: 'Closed',
};

export const TicketPriorityLabels: Record<TicketPriority, string> = {
  [TicketPriority.Low]: 'Low',
  [TicketPriority.Medium]: 'Medium',
  [TicketPriority.High]: 'High',
  [TicketPriority.Critical]: 'Critical',
};

// Matches Application/DTOs/AuthResponseDto.cs
export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
}

// Matches Application/DTOs/LoginDto.cs
export interface LoginRequest {
  email: string;
  password: string;
}

// Matches Application/DTOs/RegisterDto.cs
export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

// Matches Application/DTOs/RefreshRequestDto.cs
export interface RefreshRequest {
  refreshToken: string;
}

// Matches Application/DTOs/LogoutRequestDto.cs
export interface LogoutRequest {
  refreshToken: string;
}

// Matches Application/DTOs/Ticket/TicketResponseDto.cs
export interface Ticket {
  id: number;
  title: string;
  description: string;
  status: TicketStatus;
  priority: TicketPriority;
  createdAt: string;
  updatedAt: string;
  createdById: string;
  updatedById: string | null;
  assignedToId: string | null;
}

// Matches Application/DTOs/Ticket/CreateTicketDto.cs
export interface CreateTicketRequest {
  title: string;
  description: string;
  priority: TicketPriority;
}

// Matches Application/DTOs/Ticket/UpdateTicketDto.cs
export interface UpdateTicketRequest {
  title: string;
  description: string;
  priority: TicketPriority;
}

// Matches Application/DTOs/Ticket/TicketFilterDto.cs
export interface TicketFilter {
  page?: number;
  pageSize?: number;
  status?: TicketStatus;
  priority?: TicketPriority;
  search?: string;
  assignedTo?: string;
}

// Matches Application/DTOs/Ticket/PagedResultDto.cs
export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

// Matches Application/DTOs/UserDto.cs
export interface User {
  id: string;
  fullName: string;
  email: string;
  roles: string[];
  createdAt: string;
}

// Matches Application/DTOs/ChangeRoleDto.cs
export interface ChangeRoleRequest {
  role: string;
}

// Matches Application/DTOs/DashboardStatsDto.cs
export interface DashboardStats {
  totalTickets: number;
  openTickets: number;
  inProgressTickets: number;
  resolvedTickets: number;
  closedTickets: number;
  criticalTickets: number;
  unassignedTickets: number;
  totalUsers: number;
}

// JWT decoded payload
export interface JwtPayload {
  sub: string;
  email: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
  exp: number;
  iss: string;
  aud: string;
}

// Application auth user (derived from JWT)
export interface AuthUser {
  id: string;
  email: string;
  fullName?: string;
  role: string;
}

// API error response from GlobalExceptionMiddleware
export interface ApiError {
  statusCode: number;
  message: string;
  errors?: Record<string, string[]>;
}

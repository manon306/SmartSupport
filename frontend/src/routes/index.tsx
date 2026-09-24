import { Routes, Route, Navigate } from 'react-router-dom';
import { ProtectedRoute } from '../components/ProtectedRoute';
import { AppLayout } from '../layouts/AppLayout';

import { Login } from '../pages/auth/Login';
import { Register } from '../pages/auth/Register';
import { EmployeeDashboard } from '../pages/employee/Dashboard';
import { MyTickets } from '../pages/employee/MyTickets';
import { CreateTicket } from '../pages/employee/CreateTicket';
import { AgentDashboard } from '../pages/agent/Dashboard';
import { AllTickets } from '../pages/agent/AllTickets';
import { AdminDashboard } from '../pages/admin/Dashboard';
import { AdminTickets } from '../pages/admin/AdminTickets';
import { UserManagement } from '../pages/admin/UserManagement';
import { TicketDetails } from '../pages/shared/TicketDetails';

export const AppRoutes = () => {
  return (
    <Routes>
      {/* Public routes */}
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/" element={<Navigate to="/login" replace />} />

      {/* Protected routes wrapped in layout */}
      <Route element={<ProtectedRoute />}>
        <Route element={<AppLayout />}>
          
          {/* Employee Routes */}
          <Route element={<ProtectedRoute allowedRoles={['Employee']} />}>
            <Route path="/employee/dashboard" element={<EmployeeDashboard />} />
            <Route path="/employee/tickets" element={<MyTickets />} />
            <Route path="/employee/tickets/new" element={<CreateTicket />} />
            <Route path="/employee/tickets/:id" element={<TicketDetails />} />
          </Route>

          {/* Agent Routes */}
          <Route element={<ProtectedRoute allowedRoles={['Agent']} />}>
            <Route path="/agent/dashboard" element={<AgentDashboard />} />
            <Route path="/agent/tickets" element={<AllTickets />} />
            <Route path="/agent/tickets/:id" element={<TicketDetails />} />
          </Route>

          {/* Admin Routes */}
          <Route element={<ProtectedRoute allowedRoles={['Admin']} />}>
            <Route path="/admin/dashboard" element={<AdminDashboard />} />
            <Route path="/admin/tickets" element={<AdminTickets />} />
            <Route path="/admin/tickets/:id" element={<TicketDetails />} />
            <Route path="/admin/users" element={<UserManagement />} />
          </Route>

        </Route>
      </Route>

      {/* Catch all */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
};

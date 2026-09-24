import { Routes, Route, Navigate } from 'react-router-dom';
import { ProtectedRoute } from '../components/ProtectedRoute';
import { AppLayout } from '../layouts/AppLayout';

// Placeholder Pages
const Login = () => <div className="p-8"><h2>Login Page (Placeholder)</h2></div>;
const Register = () => <div className="p-8"><h2>Register Page (Placeholder)</h2></div>;
const EmployeeDashboard = () => <div className="p-8"><h2>Employee Dashboard (Placeholder)</h2></div>;
const AgentDashboard = () => <div className="p-8"><h2>Agent Dashboard (Placeholder)</h2></div>;
const AdminDashboard = () => <div className="p-8"><h2>Admin Dashboard (Placeholder)</h2></div>;

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
            {/* Other employee routes will go here */}
          </Route>

          {/* Agent Routes */}
          <Route element={<ProtectedRoute allowedRoles={['Agent']} />}>
            <Route path="/agent/dashboard" element={<AgentDashboard />} />
            {/* Other agent routes will go here */}
          </Route>

          {/* Admin Routes */}
          <Route element={<ProtectedRoute allowedRoles={['Admin']} />}>
            <Route path="/admin/dashboard" element={<AdminDashboard />} />
            {/* Other admin routes will go here */}
          </Route>

        </Route>
      </Route>

      {/* Catch all */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
};

import React, { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { useNavigate, Link } from 'react-router-dom';
import { Button, Input, Card } from '../../components/common';

export const Login = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await login({ email, password });
    
      const storedToken = localStorage.getItem('accessToken');
    
      if (!storedToken) {
        throw new Error('No access token found.');
      }
    
      const payload = JSON.parse(
        atob(
          storedToken
            .split('.')[1]
            .replace(/-/g, '+')
            .replace(/_/g, '/')
        )
      );
    
      const roleClaim =
        payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    
      const role = Array.isArray(roleClaim) ? roleClaim[0] : roleClaim;
    
      const dashboardByRole: Record<string, string> = {
        Admin: '/admin/dashboard',
        Agent: '/agent/dashboard',
        Employee: '/employee/dashboard',
      };
    
      navigate(dashboardByRole[role] ?? '/login', { replace: true });
    } catch (err: any) {
      if (err.response?.data?.message) {
        setError(err.response.data.message);
      } else {
        setError('Invalid credentials or server error.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Sign in to your account
        </h2>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <Card className="py-8 px-4 sm:px-10">
          <form className="space-y-6" onSubmit={handleSubmit}>
            {error && (
              <div className="bg-red-50 border border-red-200 text-red-600 px-4 py-3 rounded-md text-sm">
                {error}
              </div>
            )}
            
            <Input
              label="Email address"
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />

            <Input
              label="Password"
              type="password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />

            <div>
              <Button type="submit" className="w-full" isLoading={isLoading}>
                Sign in
              </Button>
            </div>
          </form>
          
          <div className="mt-6 text-center text-sm">
            <Link to="/register" className="font-medium text-primary hover:text-primary-focus">
              Don't have an account? Register
            </Link>
          </div>
        </Card>
      </div>
    </div>
  );
};

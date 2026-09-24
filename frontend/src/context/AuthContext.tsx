import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import apiClient from '../api/axios';
import type { AuthUser, LoginRequest, AuthResponse, RegisterRequest } from '../types';

interface AuthContextType {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (credentials: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Helper to decode JWT
function parseJwt(token: string): any {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
    return JSON.parse(jsonPayload);
  } catch (e) {
    return null;
  }
}

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);

  useEffect(() => {
    // Check if user is logged in on mount
    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      try {
        const decoded = parseJwt(accessToken);
        if (decoded) {
          // Extract role claim based on Microsoft schemas
          const roleClaim = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
          const role = Array.isArray(roleClaim) ? roleClaim[0] : roleClaim;
          const nameClaim = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
          
          setUser({
            id: decoded.sub,
            email: decoded.email,
            fullName: nameClaim || decoded.email,
            role: role
          });
        }
      } catch (error) {
        console.error('Error decoding token', error);
      }
    }
    setIsLoading(false);
  }, []);

  const login = async (credentials: LoginRequest) => {
    const response = await apiClient.post<AuthResponse>('/Auth/login', credentials);
    const { accessToken, refreshToken } = response.data;
    
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    
    const decoded = parseJwt(accessToken);
    const roleClaim = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    const role = Array.isArray(roleClaim) ? roleClaim[0] : roleClaim;
    
    setUser({
      id: decoded.sub,
      email: decoded.email,
      role: role
    });
  };

  const register = async (data: RegisterRequest) => {
    const response = await apiClient.post<AuthResponse>('/Auth/register', data);
    const { accessToken, refreshToken } = response.data;
    
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    
    const decoded = parseJwt(accessToken);
    const roleClaim = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    const role = Array.isArray(roleClaim) ? roleClaim[0] : roleClaim;
    
    setUser({
      id: decoded.sub,
      email: decoded.email,
      role: role
    });
  };

  const logout = async () => {
    try {
      const refreshToken = localStorage.getItem('refreshToken');
      if (refreshToken) {
        await apiClient.post('/Auth/logout', { refreshToken });
      }
    } catch (error) {
      console.error('Logout error', error);
    } finally {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      setUser(null);
    }
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated: !!user, isLoading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

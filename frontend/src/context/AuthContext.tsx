import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';

import apiClient from '../api/axios';
import type {
  AuthUser,
  LoginRequest,
  AuthResponse,
  RegisterRequest,
} from '../types';

interface AuthContextType {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (credentials: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Decode JWT
function parseJwt(token: string): any {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');

    const jsonPayload = decodeURIComponent(
      window
        .atob(base64)
        .split('')
        .map((c) => {
          return (
            '%' +
            ('00' + c.charCodeAt(0).toString(16)).slice(-2)
          );
        })
        .join('')
    );

    return JSON.parse(jsonPayload);
  } catch {
    return null;
  }
}

// Extract user information from JWT
function getUserFromToken(token: string): AuthUser | null {
  const decoded = parseJwt(token);

  if (!decoded) {
    return null;
  }

  const id =
    decoded[
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
    ];

  const email =
    decoded[
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'
    ];

  const roleClaim =
    decoded[
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
    ];

  const role = Array.isArray(roleClaim)
    ? roleClaim[0]
    : roleClaim;

  const name =
    decoded[
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
    ];

  if (!id || !email || !role) {
    return null;
  }

  return {
    id,
    email,
    fullName: name || email,
    role,
  };
}

export const AuthProvider = ({
  children,
}: {
  children: ReactNode;
}) => {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);

  // Restore authentication after refresh
  useEffect(() => {
    const accessToken = localStorage.getItem('accessToken');

    if (accessToken) {
      const userFromToken = getUserFromToken(accessToken);

      if (userFromToken) {
        setUser(userFromToken);
      } else {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
      }
    }

    setIsLoading(false);
  }, []);

  const login = async (credentials: LoginRequest) => {
    const response = await apiClient.post<AuthResponse>(
      '/Auth/login',
      credentials
    );

    const { accessToken, refreshToken } = response.data;

    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);

    const userFromToken = getUserFromToken(accessToken);

    if (!userFromToken) {
      throw new Error('Invalid authentication token.');
    }

    setUser(userFromToken);
  };

  const register = async (data: RegisterRequest) => {
    const response = await apiClient.post<AuthResponse>(
      '/Auth/register',
      data
    );

    const { accessToken, refreshToken } = response.data;

    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);

    const userFromToken = getUserFromToken(accessToken);

    if (!userFromToken) {
      throw new Error('Invalid authentication token.');
    }

    setUser(userFromToken);
  };

  const logout = async () => {
    try {
      const refreshToken = localStorage.getItem('refreshToken');

      if (refreshToken) {
        await apiClient.post('/Auth/logout', {
          refreshToken,
        });
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
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user,
        isLoading,
        login,
        register,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);

  if (context === undefined) {
    throw new Error(
      'useAuth must be used within an AuthProvider'
    );
  }

  return context;
};
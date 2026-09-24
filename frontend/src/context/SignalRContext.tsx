import { createContext, useContext, useEffect, useState } from 'react';
import type { ReactNode } from 'react';
import * as signalR from '@microsoft/signalr';
import { useAuth } from './AuthContext';

interface Notification {
  id: string;
  message: string;
  timestamp: Date;
  read: boolean;
}

interface SignalRContextType {
  notifications: Notification[];
  unreadCount: number;
  markAsRead: (id: string) => void;
  markAllAsRead: () => void;
  isConnected: boolean;
}

const SignalRContext = createContext<SignalRContextType | undefined>(undefined);

export const SignalRProvider = ({ children }: { children: ReactNode }) => {
  const { isAuthenticated } = useAuth();
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    if (!isAuthenticated) {
      if (connection) {
        connection.stop();
        setConnection(null);
        setIsConnected(false);
      }
      return;
    }

    const hubUrl = import.meta.env.VITE_SIGNALR_HUB_URL || 'https://localhost:7250/hubs/notifications';
    
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => localStorage.getItem('accessToken') || ''
      })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, [isAuthenticated]);

  useEffect(() => {
    if (connection) {
      connection.start()
        .then(() => {
          console.log('Connected to SignalR hub');
          setIsConnected(true);
          
          connection.on('ReceiveNotification', (message: string) => {
            const newNotification: Notification = {
              id: Date.now().toString(),
              message,
              timestamp: new Date(),
              read: false
            };
            setNotifications(prev => [newNotification, ...prev]);
            
            // Play a subtle sound or trigger toast here if desired
          });
        })
        .catch(e => console.error('Connection failed: ', e));

      connection.onreconnecting(() => setIsConnected(false));
      connection.onreconnected(() => setIsConnected(true));
      connection.onclose(() => setIsConnected(false));

      return () => {
        connection.off('ReceiveNotification');
        connection.stop();
      };
    }
  }, [connection]);

  const markAsRead = (id: string) => {
    setNotifications(prev => 
      prev.map(n => n.id === id ? { ...n, read: true } : n)
    );
  };

  const markAllAsRead = () => {
    setNotifications(prev => prev.map(n => ({ ...n, read: true })));
  };

  const unreadCount = notifications.filter(n => !n.read).length;

  return (
    <SignalRContext.Provider value={{ notifications, unreadCount, markAsRead, markAllAsRead, isConnected }}>
      {children}
    </SignalRContext.Provider>
  );
};

export const useSignalR = () => {
  const context = useContext(SignalRContext);
  if (context === undefined) {
    throw new Error('useSignalR must be used within a SignalRProvider');
  }
  return context;
};

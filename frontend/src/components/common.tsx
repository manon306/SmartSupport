import React from 'react';
import type { ButtonHTMLAttributes, InputHTMLAttributes } from 'react';
import { TicketPriority, TicketStatus } from '../types';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger' | 'ghost';
  isLoading?: boolean;
}

export const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className = '', variant = 'primary', isLoading, children, disabled, ...props }, ref) => {
    const baseStyles = 'inline-flex items-center justify-center px-4 py-2 text-sm font-medium rounded-md transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed';
    
    const variants = {
      primary: 'bg-primary text-white hover:bg-primary-focus focus:ring-primary',
      secondary: 'bg-gray-200 text-gray-900 hover:bg-gray-300 focus:ring-gray-500',
      danger: 'bg-red-600 text-white hover:bg-red-700 focus:ring-red-500',
      ghost: 'bg-transparent text-gray-700 hover:bg-gray-100 focus:ring-gray-500',
    };

    return (
      <button
        ref={ref}
        className={`${baseStyles} ${variants[variant]} ${className}`}
        disabled={disabled || isLoading}
        {...props}
      >
        {isLoading && (
          <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-current" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
        )}
        {children}
      </button>
    );
  }
);
Button.displayName = 'Button';

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
}

export const Input = React.forwardRef<HTMLInputElement, InputProps>(
  ({ className = '', label, error, ...props }, ref) => {
    return (
      <div className="mb-4">
        {label && <label className="block text-sm font-medium text-gray-700 mb-1">{label}</label>}
        <input
          ref={ref}
          className={`block w-full rounded-md border-gray-300 shadow-sm focus:border-primary focus:ring-primary sm:text-sm border p-2 ${error ? 'border-red-500' : ''} ${className}`}
          {...props}
        />
        {error && <p className="mt-1 text-sm text-red-600">{error}</p>}
      </div>
    );
  }
);
Input.displayName = 'Input';

export const Card = ({ children, className = '' }: { children: React.ReactNode; className?: string }) => (
  <div className={`bg-white shadow rounded-lg overflow-hidden ${className}`}>
    {children}
  </div>
);

export const StatusBadge = ({ status }: { status: TicketStatus }) => {
  const styles = {
    [TicketStatus.Open]: 'bg-blue-100 text-blue-800',
    [TicketStatus.InProgress]: 'bg-yellow-100 text-yellow-800',
    [TicketStatus.Resolved]: 'bg-green-100 text-green-800',
    [TicketStatus.Closed]: 'bg-gray-100 text-gray-800',
  };

  const labels = {
    [TicketStatus.Open]: 'Open',
    [TicketStatus.InProgress]: 'In Progress',
    [TicketStatus.Resolved]: 'Resolved',
    [TicketStatus.Closed]: 'Closed',
  };

  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${styles[status]}`}>
      {labels[status]}
    </span>
  );
};

export const PriorityBadge = ({ priority }: { priority: TicketPriority }) => {
  const styles = {
    [TicketPriority.Low]: 'bg-gray-100 text-gray-800',
    [TicketPriority.Medium]: 'bg-blue-100 text-blue-800',
    [TicketPriority.High]: 'bg-orange-100 text-orange-800',
    [TicketPriority.Critical]: 'bg-red-100 text-red-800',
  };

  const labels = {
    [TicketPriority.Low]: 'Low',
    [TicketPriority.Medium]: 'Medium',
    [TicketPriority.High]: 'High',
    [TicketPriority.Critical]: 'Critical',
  };

  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${styles[priority]}`}>
      {labels[priority]}
    </span>
  );
};

export const LoadingState = () => (
  <div className="flex justify-center items-center py-12">
    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
  </div>
);

export const EmptyState = ({ message = "No records found." }: { message?: string }) => (
  <div className="text-center py-12 bg-white rounded-lg shadow border border-gray-200">
    <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" aria-hidden="true">
      <path vectorEffect="non-scaling-stroke" strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 13h6m-3-3v6m-9 1V7a2 2 0 012-2h6l2 2h6a2 2 0 012 2v8a2 2 0 01-2 2H5a2 2 0 01-2-2z" />
    </svg>
    <h3 className="mt-2 text-sm font-medium text-gray-900">No data</h3>
    <p className="mt-1 text-sm text-gray-500">{message}</p>
  </div>
);

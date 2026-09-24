import { useState, useEffect } from 'react';
import { adminApi } from '../../api/admin';
import type { DashboardStats } from '../../types';
import { Card, LoadingState } from '../../components/common';
import { Link } from 'react-router-dom';

export const AdminDashboard = () => {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadStats = async () => {
      try {
        const data = await adminApi.getDashboardStats();
        setStats(data);
      } catch (err) {
        console.error('Failed to load dashboard stats', err);
      } finally {
        setIsLoading(false);
      }
    };
    loadStats();
  }, []);

  if (isLoading) return <LoadingState />;
  if (!stats) return <div className="p-4 text-red-500">Failed to load statistics.</div>;

  const statCards = [
    { label: 'Total Tickets', value: stats.totalTickets, color: 'bg-blue-50 text-blue-700 border-blue-200' },
    { label: 'Open', value: stats.openTickets, color: 'bg-gray-50 text-gray-700 border-gray-200' },
    { label: 'In Progress', value: stats.inProgressTickets, color: 'bg-yellow-50 text-yellow-700 border-yellow-200' },
    { label: 'Resolved', value: stats.resolvedTickets, color: 'bg-green-50 text-green-700 border-green-200' },
    { label: 'Closed', value: stats.closedTickets, color: 'bg-slate-50 text-slate-700 border-slate-200' },
    { label: 'Critical', value: stats.criticalTickets, color: 'bg-red-50 text-red-700 border-red-200' },
    { label: 'Unassigned', value: stats.unassignedTickets, color: 'bg-orange-50 text-orange-700 border-orange-200' },
    { label: 'Total Users', value: stats.totalUsers, color: 'bg-indigo-50 text-indigo-700 border-indigo-200' },
  ];

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Admin Dashboard</h1>

      <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-4">
        {statCards.map((stat, i) => (
          <Card key={i} className={`p-6 border ${stat.color}`}>
            <p className="text-sm font-medium opacity-80">{stat.label}</p>
            <p className="mt-2 text-3xl font-bold">{stat.value}</p>
          </Card>
        ))}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mt-8">
        <Card className="p-6">
          <h3 className="text-lg font-medium mb-4">Quick Links</h3>
          <div className="space-y-3 flex flex-col">
            <Link to="/admin/tickets" className="text-primary hover:underline">Manage All Tickets</Link>
            <Link to="/admin/users" className="text-primary hover:underline">Manage Users & Roles</Link>
          </div>
        </Card>
      </div>
    </div>
  );
};

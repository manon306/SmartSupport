import { useState, useEffect } from 'react';
import { adminApi } from '../../api/admin';
import type { User, ChangeRoleRequest } from '../../types';
import { Card, LoadingState } from '../../components/common';

export const UserManagement = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [updatingUserId, setUpdatingUserId] = useState<string | null>(null);

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    setIsLoading(true);
    try {
      const data = await adminApi.getUsers();
      setUsers(data);
    } catch (err) {
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleRoleChange = async (userId: string, newRole: string) => {
    setUpdatingUserId(userId);
    try {
      const request: ChangeRoleRequest = { role: newRole };
      const updatedUser = await adminApi.changeUserRole(userId, request);
      setUsers(users.map(u => u.id === userId ? updatedUser : u));
    } catch (err: any) {
      alert(err.response?.data?.message || 'Failed to update role');
    } finally {
      setUpdatingUserId(null);
    }
  };

  if (isLoading) return <LoadingState />;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">User Management</h1>

      <Card>
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-300">
            <thead className="bg-gray-50">
              <tr>
                <th className="py-3.5 pl-4 pr-3 text-left text-sm font-semibold text-gray-900">Name</th>
                <th className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Email</th>
                <th className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Created At</th>
                <th className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Current Role</th>
                <th className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Change Role</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200 bg-white">
              {users.map(user => {
                const currentRole = user.roles[0] || 'Employee';
                return (
                  <tr key={user.id}>
                    <td className="whitespace-nowrap py-4 pl-4 pr-3 text-sm font-medium text-gray-900">{user.fullName}</td>
                    <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{user.email}</td>
                    <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{new Date(user.createdAt).toLocaleDateString()}</td>
                    <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                      <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                        {currentRole}
                      </span>
                    </td>
                    <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                      <select
                        className="block w-full rounded-md border-gray-300 shadow-sm border p-1 text-sm disabled:opacity-50"
                        value={currentRole}
                        disabled={updatingUserId === user.id}
                        onChange={e => handleRoleChange(user.id, e.target.value)}
                      >
                        <option value="Employee">Employee</option>
                        <option value="Agent">Agent</option>
                        <option value="Admin">Admin</option>
                      </select>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  );
};

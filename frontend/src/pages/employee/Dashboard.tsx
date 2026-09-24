import { Card } from '../../components/common';
import { Link } from 'react-router-dom';

export const EmployeeDashboard = () => {
  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Employee Dashboard</h1>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <Card className="p-6">
          <h3 className="text-lg font-medium mb-4">Quick Links</h3>
          <div className="space-y-3 flex flex-col">
            <Link to="/employee/tickets" className="text-primary hover:underline">My Tickets</Link>
            <Link to="/employee/tickets/new" className="text-primary hover:underline">Create a New Ticket</Link>
          </div>
        </Card>
      </div>
    </div>
  );
};

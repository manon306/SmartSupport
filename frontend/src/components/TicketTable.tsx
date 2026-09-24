import { Link } from 'react-router-dom';
import type { Ticket } from '../types';
import { StatusBadge, PriorityBadge } from './common';

interface TicketTableProps {
  tickets: Ticket[];
  basePath: string; // e.g. '/employee/tickets' or '/agent/tickets'
  showAssignedTo?: boolean;
}

export const TicketTable = ({ tickets, basePath, showAssignedTo = false }: TicketTableProps) => {
  if (tickets.length === 0) {
    return null; // Handled by EmptyState in parent
  }

  return (
    <div className="overflow-x-auto shadow ring-1 ring-black ring-opacity-5 rounded-lg">
      <table className="min-w-full divide-y divide-gray-300">
        <thead className="bg-gray-50">
          <tr>
            <th scope="col" className="py-3.5 pl-4 pr-3 text-left text-sm font-semibold text-gray-900 sm:pl-6">ID</th>
            <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Title</th>
            <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Status</th>
            <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Priority</th>
            <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Created</th>
            {showAssignedTo && <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Assigned To</th>}
            <th scope="col" className="relative py-3.5 pl-3 pr-4 sm:pr-6">
              <span className="sr-only">View</span>
            </th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-200 bg-white">
          {tickets.map((ticket) => (
            <tr key={ticket.id} className="hover:bg-gray-50">
              <td className="whitespace-nowrap py-4 pl-4 pr-3 text-sm font-medium text-gray-900 sm:pl-6">
                #{ticket.id}
              </td>
              <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                <div className="max-w-xs truncate" title={ticket.title}>{ticket.title}</div>
              </td>
              <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                <StatusBadge status={ticket.status} />
              </td>
              <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                <PriorityBadge priority={ticket.priority} />
              </td>
              <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                {new Date(ticket.createdAt).toLocaleDateString()}
              </td>
              {showAssignedTo && (
                <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                  {ticket.assignedToId ? ticket.assignedToId : <span className="text-gray-400 italic">Unassigned</span>}
                </td>
              )}
              <td className="relative whitespace-nowrap py-4 pl-3 pr-4 text-right text-sm font-medium sm:pr-6">
                <Link to={`${basePath}/${ticket.id}`} className="text-primary hover:text-primary-focus">
                  View<span className="sr-only">, {ticket.title}</span>
                </Link>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

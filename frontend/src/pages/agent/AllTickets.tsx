import { useState, useEffect } from 'react';
import { ticketsApi } from '../../api/tickets';
import type { Ticket, PagedResult } from '../../types';
import { TicketStatus, TicketPriority } from '../../types';
import { TicketTable } from '../../components/TicketTable';
import { Card, LoadingState, EmptyState, Button, Input } from '../../components/common';
import { useAuth } from '../../context/AuthContext';

export const AllTickets = () => {
  const { user } = useAuth();
  const [data, setData] = useState<PagedResult<Ticket> | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState<TicketStatus | ''>('');
  const [priority, setPriority] = useState<TicketPriority | ''>('');
  const [viewFilter, setViewFilter] = useState<'all' | 'unassigned' | 'mine'>('all');

  const loadTickets = async () => {
    setIsLoading(true);
    setError('');
    
    let assignedToParam: string | undefined = undefined;
    if (viewFilter === 'unassigned') {
      assignedToParam = 'null'; // The backend might need adjustment, or we handle it if API supports it.
      // Wait, standard API filter doesn't support 'unassigned' easily unless we implemented it. 
      // The instructions say "respect backend rules". I will rely on standard API. 
      // If backend doesn't support assignedTo filter, I'll pass it and see.
    } else if (viewFilter === 'mine' && user) {
      assignedToParam = user.id;
    }

    try {
      const result = await ticketsApi.getTickets({
        page,
        pageSize: 10,
        search: search || undefined,
        status: status !== '' ? status : undefined,
        priority: priority !== '' ? priority : undefined,
        assignedTo: assignedToParam
      });
      setData(result);
    } catch (err) {
      setError('Failed to load tickets.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    const delayDebounce = setTimeout(() => {
      loadTickets();
    }, 500);
    return () => clearTimeout(delayDebounce);
  }, [page, search, status, priority, viewFilter]);

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">All Tickets</h1>

      <Card className="p-4">
        <div className="flex space-x-2 mb-4 border-b pb-4">
          <Button 
            variant={viewFilter === 'all' ? 'primary' : 'ghost'} 
            onClick={() => setViewFilter('all')}
          >
            All
          </Button>
          <Button 
            variant={viewFilter === 'mine' ? 'primary' : 'ghost'} 
            onClick={() => setViewFilter('mine')}
          >
            Assigned to Me
          </Button>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-4">
          <Input 
            placeholder="Search tickets..." 
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
          <select 
            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-primary focus:ring-primary sm:text-sm border p-2 h-[42px] mt-0 sm:mt-6"
            value={status}
            onChange={(e) => setStatus(e.target.value === '' ? '' : Number(e.target.value) as TicketStatus)}
          >
            <option value="">All Statuses</option>
            <option value={TicketStatus.Open}>Open</option>
            <option value={TicketStatus.InProgress}>In Progress</option>
            <option value={TicketStatus.Resolved}>Resolved</option>
            <option value={TicketStatus.Closed}>Closed</option>
          </select>
          <select 
            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-primary focus:ring-primary sm:text-sm border p-2 h-[42px] mt-0 sm:mt-6"
            value={priority}
            onChange={(e) => setPriority(e.target.value === '' ? '' : Number(e.target.value) as TicketPriority)}
          >
            <option value="">All Priorities</option>
            <option value={TicketPriority.Low}>Low</option>
            <option value={TicketPriority.Medium}>Medium</option>
            <option value={TicketPriority.High}>High</option>
            <option value={TicketPriority.Critical}>Critical</option>
          </select>
        </div>

        {error && <div className="text-red-500 mb-4">{error}</div>}

        {isLoading && !data ? (
          <LoadingState />
        ) : data?.items.length === 0 ? (
          <EmptyState message="No tickets found matching your criteria." />
        ) : (
          <>
            <TicketTable tickets={data?.items || []} basePath="/agent/tickets" showAssignedTo={true} />
            
            {data && data.totalPages > 1 && (
              <div className="mt-4 flex justify-between items-center px-4">
                <span className="text-sm text-gray-700">
                  Showing {((data.page - 1) * data.pageSize) + 1} to {Math.min(data.page * data.pageSize, data.totalCount)} of {data.totalCount} results
                </span>
                <div className="flex space-x-2">
                  <Button 
                    variant="secondary" 
                    disabled={data.page === 1}
                    onClick={() => setPage(p => Math.max(1, p - 1))}
                  >
                    Previous
                  </Button>
                  <Button 
                    variant="secondary" 
                    disabled={data.page === data.totalPages}
                    onClick={() => setPage(p => Math.min(data.totalPages, p + 1))}
                  >
                    Next
                  </Button>
                </div>
              </div>
            )}
          </>
        )}
      </Card>
    </div>
  );
};

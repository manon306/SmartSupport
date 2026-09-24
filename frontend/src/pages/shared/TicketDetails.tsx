import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ticketsApi } from '../../api/tickets';
import { adminApi } from '../../api/admin';
import type { Ticket, User } from '../../types';
import { TicketStatus, TicketPriority } from '../../types';
import { useAuth } from '../../context/AuthContext';
import { Button, Input, Card, StatusBadge, PriorityBadge, LoadingState } from '../../components/common';

export const TicketDetails = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user } = useAuth();
  
  const [ticket, setTicket] = useState<Ticket | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  
  // Edit State
  const [isEditing, setIsEditing] = useState(false);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState<TicketPriority>(TicketPriority.Medium);
  const [isSaving, setIsSaving] = useState(false);

  // Admin Assign State
  const [agents, setAgents] = useState<User[]>([]);
  const [selectedAgentId, setSelectedAgentId] = useState('');
  const [isAssigning, setIsAssigning] = useState(false);

  // Status Change State
  const [isChangingStatus, setIsChangingStatus] = useState(false);

  useEffect(() => {
    loadTicket();
    if (user?.role === 'Admin') {
      loadAgents();
    }
  }, [id, user]);

  const loadTicket = async () => {
    try {
      const data = await ticketsApi.getTicketById(Number(id));
      setTicket(data);
      setTitle(data.title);
      setDescription(data.description);
      setPriority(data.priority);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load ticket.');
    } finally {
      setIsLoading(false);
    }
  };

  const loadAgents = async () => {
    try {
      const data = await adminApi.getAgents();
      setAgents(data);
    } catch (err) {
      console.error('Failed to load agents', err);
    }
  };

  const handleUpdate = async () => {
    setIsSaving(true);
    try {
      const updated = await ticketsApi.updateTicket(Number(id), { title, description, priority });
      setTicket(updated);
      setIsEditing(false);
    } catch (err: any) {
      alert(err.response?.data?.message || 'Update failed');
    } finally {
      setIsSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Are you sure you want to delete this ticket?')) return;
    try {
      await ticketsApi.deleteTicket(Number(id));
      navigate('/admin/tickets');
    } catch (err: any) {
      alert(err.response?.data?.message || 'Delete failed');
    }
  };

  const handleAssign = async () => {
    if (!selectedAgentId) return;
    setIsAssigning(true);
    try {
      const updated = await ticketsApi.assignTicket(Number(id), selectedAgentId);
      setTicket(updated);
      setSelectedAgentId('');
    } catch (err: any) {
      alert(err.response?.data?.message || 'Assignment failed');
    } finally {
      setIsAssigning(false);
    }
  };

  const handleStatusChange = async (newStatus: TicketStatus) => {
    setIsChangingStatus(true);
    try {
      const updated = await ticketsApi.changeStatus(Number(id), newStatus);
      setTicket(updated);
    } catch (err: any) {
      alert(err.response?.data?.message || 'Status change failed');
    } finally {
      setIsChangingStatus(false);
    }
  };

  if (isLoading) return <LoadingState />;
  if (error || !ticket) return <div className="p-8 text-red-500">{error || 'Ticket not found'}</div>;

  const isCreator = user?.id === ticket.createdById;
  const isAssignedToMe = user?.id === ticket.assignedToId;
  const isAdmin = user?.role === 'Admin';
  const isAgent = user?.role === 'Agent';
  const isEmployee = user?.role === 'Employee';

  const canEdit = ticket.status !== TicketStatus.Closed && (
    isAdmin || 
    (isEmployee && isCreator) || 
    (isAgent && isAssignedToMe)
  );

  return (
    <div className="max-w-4xl mx-auto space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-4">
          <button onClick={() => navigate(-1)} className="text-gray-500 hover:text-gray-700">
            &larr; Back
          </button>
          <h1 className="text-2xl font-bold text-gray-900">Ticket #{ticket.id}</h1>
        </div>
        <div className="flex space-x-2">
          {canEdit && !isEditing && (
            <Button variant="secondary" onClick={() => setIsEditing(true)}>Edit</Button>
          )}
          {isAdmin && (
            <Button variant="danger" onClick={handleDelete}>Delete</Button>
          )}
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="md:col-span-2 space-y-6">
          <Card className="p-6">
            {isEditing ? (
              <div className="space-y-4">
                <Input label="Title" value={title} onChange={e => setTitle(e.target.value)} />
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Description</label>
                  <textarea
                    rows={5}
                    className="block w-full rounded-md border-gray-300 shadow-sm border p-2"
                    value={description}
                    onChange={e => setDescription(e.target.value)}
                  />
                </div>
                {!isEmployee && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Priority</label>
                    <select
                      className="block w-full rounded-md border-gray-300 shadow-sm border p-2"
                      value={priority}
                      onChange={e => setPriority(Number(e.target.value) as TicketPriority)}
                    >
                      <option value={TicketPriority.Low}>Low</option>
                      <option value={TicketPriority.Medium}>Medium</option>
                      <option value={TicketPriority.High}>High</option>
                      <option value={TicketPriority.Critical}>Critical</option>
                    </select>
                  </div>
                )}
                <div className="flex justify-end space-x-2 pt-4">
                  <Button variant="ghost" onClick={() => {
                    setIsEditing(false);
                    setTitle(ticket.title);
                    setDescription(ticket.description);
                    setPriority(ticket.priority);
                  }}>Cancel</Button>
                  <Button onClick={handleUpdate} isLoading={isSaving}>Save Changes</Button>
                </div>
              </div>
            ) : (
              <div>
                <h2 className="text-xl font-semibold mb-4">{ticket.title}</h2>
                <div className="prose max-w-none text-gray-700 whitespace-pre-wrap">
                  {ticket.description}
                </div>
              </div>
            )}
          </Card>
        </div>

        <div className="space-y-6">
          <Card className="p-6">
            <h3 className="text-lg font-medium border-b pb-2 mb-4">Details</h3>
            <dl className="space-y-4">
              <div>
                <dt className="text-sm font-medium text-gray-500">Status</dt>
                <dd className="mt-1"><StatusBadge status={ticket.status} /></dd>
              </div>
              <div>
                <dt className="text-sm font-medium text-gray-500">Priority</dt>
                <dd className="mt-1"><PriorityBadge priority={ticket.priority} /></dd>
              </div>
              <div>
                <dt className="text-sm font-medium text-gray-500">Created At</dt>
                <dd className="mt-1 text-sm text-gray-900">{new Date(ticket.createdAt).toLocaleString()}</dd>
              </div>
              <div>
                <dt className="text-sm font-medium text-gray-500">Assigned To</dt>
                <dd className="mt-1 text-sm text-gray-900">
                  {ticket.assignedToId ? ticket.assignedToId : <span className="italic text-gray-400">Unassigned</span>}
                </dd>
              </div>
            </dl>
          </Card>

          {/* Agent/Admin Actions */}
          {(isAdmin || (isAgent && isAssignedToMe)) && ticket.status !== TicketStatus.Closed && (
            <Card className="p-6">
              <h3 className="text-lg font-medium border-b pb-2 mb-4">Workflow</h3>
              <div className="space-y-3">
                {ticket.status === TicketStatus.Open && (
                  <Button className="w-full" isLoading={isChangingStatus} onClick={() => handleStatusChange(TicketStatus.InProgress)}>
                    Start Progress
                  </Button>
                )}
                {ticket.status === TicketStatus.InProgress && (
                  <Button className="w-full bg-green-600 hover:bg-green-700" isLoading={isChangingStatus} onClick={() => handleStatusChange(TicketStatus.Resolved)}>
                    Mark as Resolved
                  </Button>
                )}
                {ticket.status === TicketStatus.Resolved && (
                  <Button className="w-full bg-gray-600 hover:bg-gray-700" isLoading={isChangingStatus} onClick={() => handleStatusChange(TicketStatus.Closed)}>
                    Close Ticket
                  </Button>
                )}
              </div>
            </Card>
          )}

          {/* Admin Assign */}
          {isAdmin && (
            <Card className="p-6">
              <h3 className="text-lg font-medium border-b pb-2 mb-4">Assign Agent</h3>
              <div className="space-y-3">
                <select
                  className="block w-full rounded-md border-gray-300 shadow-sm border p-2 text-sm"
                  value={selectedAgentId}
                  onChange={e => setSelectedAgentId(e.target.value)}
                >
                  <option value="">Select Agent...</option>
                  {agents.map(a => (
                    <option key={a.id} value={a.id}>{a.fullName} ({a.email})</option>
                  ))}
                </select>
                <Button 
                  className="w-full" 
                  disabled={!selectedAgentId} 
                  isLoading={isAssigning}
                  onClick={handleAssign}
                >
                  Assign
                </Button>
              </div>
            </Card>
          )}
        </div>
      </div>
    </div>
  );
};

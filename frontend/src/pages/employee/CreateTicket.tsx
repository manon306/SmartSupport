import { useState } from 'react';
import { ticketsApi } from '../../api/tickets';
import { TicketPriority } from '../../types';
import { useNavigate } from 'react-router-dom';
import { Button, Input, Card } from '../../components/common';

export const CreateTicket = () => {
  const navigate = useNavigate();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState<TicketPriority>(TicketPriority.Medium);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    try {
      await ticketsApi.createTicket({ title, description, priority });
      navigate('/employee/tickets');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to create ticket.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div className="flex items-center space-x-4">
        <button onClick={() => navigate(-1)} className="text-gray-500 hover:text-gray-700">
          &larr; Back
        </button>
        <h1 className="text-2xl font-bold text-gray-900">Create New Ticket</h1>
      </div>

      <Card className="p-6">
        <form onSubmit={handleSubmit} className="space-y-6">
          {error && <div className="text-red-500 bg-red-50 p-3 rounded">{error}</div>}

          <Input
            label="Title"
            required
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Brief summary of the issue"
          />

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Description</label>
            <textarea
              required
              rows={5}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-primary focus:ring-primary sm:text-sm border p-2"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Detailed explanation..."
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Priority</label>
            <select
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-primary focus:ring-primary sm:text-sm border p-2"
              value={priority}
              onChange={(e) => setPriority(Number(e.target.value) as TicketPriority)}
            >
              <option value={TicketPriority.Low}>Low</option>
              <option value={TicketPriority.Medium}>Medium</option>
              <option value={TicketPriority.High}>High</option>
              <option value={TicketPriority.Critical}>Critical</option>
            </select>
          </div>

          <div className="flex justify-end space-x-3 pt-4 border-t">
            <Button type="button" variant="ghost" onClick={() => navigate(-1)}>Cancel</Button>
            <Button type="submit" isLoading={isLoading}>Create Ticket</Button>
          </div>
        </form>
      </Card>
    </div>
  );
};

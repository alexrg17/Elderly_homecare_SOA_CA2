import { useState, useEffect } from 'react';
import MainLayout from '../components/layout/MainLayout.jsx';
import PageHeader from '../components/common/PageHeader.jsx';
import Card from '../components/common/Card.jsx';
import Table from '../components/common/Table.jsx';
import Badge from '../components/common/Badge.jsx';
import Button from '../components/common/Button.jsx';
import LoadingSpinner from '../components/common/LoadingSpinner.jsx';
import StatCard from '../components/common/StatCard.jsx';
import { FaBell, FaExclamationTriangle, FaCheckCircle, FaTrash, FaEdit } from 'react-icons/fa';
import alertsService from '../services/alertsService';
import { useAuth } from '../contexts/AuthContext';

const Alerts = () => {
  const { user } = useAuth();
  const [alerts, setAlerts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filter, setFilter] = useState('all'); // all, unresolved, resolved
  const [showResolveModal, setShowResolveModal] = useState(false);
  const [selectedAlert, setSelectedAlert] = useState(null);
  const [resolutionNotes, setResolutionNotes] = useState('');

  useEffect(() => {
    fetchAlerts();
  }, []);

  const fetchAlerts = async () => {
    try {
      setLoading(true);
      const data = await alertsService.getAll();
      setAlerts(data);
      setError(null);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to fetch alerts');
      console.error('Error fetching alerts:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleResolve = (alert) => {
    setSelectedAlert(alert);
    setResolutionNotes('');
    setShowResolveModal(true);
  };

  const handleResolveSubmit = async (e) => {
    e.preventDefault();
    
    if (!user?.id) {
      alert('User information not available');
      return;
    }

    try {
      await alertsService.resolve(selectedAlert.id, user.id, resolutionNotes);
      setShowResolveModal(false);
      fetchAlerts();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to resolve alert');
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this alert?')) return;
    
    try {
      await alertsService.delete(id);
      fetchAlerts();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to delete alert');
    }
  };

  const getSeverityVariant = (severity) => {
    switch (severity.toLowerCase()) {
      case 'critical': return 'error';
      case 'high': return 'warning';
      case 'medium': return 'info';
      case 'low': return 'secondary';
      default: return 'secondary';
    }
  };

  const formatDateTime = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleString();
  };

  const filteredAlerts = alerts.filter(alert => {
    if (filter === 'unresolved') return !alert.isResolved;
    if (filter === 'resolved') return alert.isResolved;
    return true;
  });

  const stats = {
    total: alerts.length,
    unresolved: alerts.filter(a => !a.isResolved).length,
    resolved: alerts.filter(a => a.isResolved).length,
    critical: alerts.filter(a => !a.isResolved && a.severity.toLowerCase() === 'critical').length
  };

  const columns = [
    { 
      header: 'Created', 
      accessor: (row) => (
        <span className="text-sm">{formatDateTime(row.createdAt)}</span>
      )
    },
    { header: 'Room', accessor: 'roomNumber' },
    { 
      header: 'Type', 
      accessor: 'alertType'
    },
    {
      header: 'Severity',
      accessor: (row) => (
        <Badge variant={getSeverityVariant(row.severity)}>
          {row.severity}
        </Badge>
      )
    },
    { 
      header: 'Message', 
      accessor: (row) => (
        <span className="text-sm">{row.message}</span>
      )
    },
    {
      header: 'Status',
      accessor: (row) => (
        row.isResolved ? (
          <Badge variant="success">
            <FaCheckCircle className="inline mr-1" />
            Resolved
          </Badge>
        ) : (
          <Badge variant="error">
            <FaExclamationTriangle className="inline mr-1" />
            Active
          </Badge>
        )
      )
    },
    {
      header: 'Resolved By',
      accessor: (row) => row.resolvedByUsername || '-'
    },
    {
      header: 'Resolution Notes',
      accessor: (row) => row.isResolved && row.resolutionNotes ? (
        <span className="text-sm text-gray-600" title={row.resolutionNotes}>
          {row.resolutionNotes.length > 50 ? row.resolutionNotes.substring(0, 50) + '...' : row.resolutionNotes}
        </span>
      ) : (
        <span className="text-gray-400">-</span>
      )
    },
    {
      header: 'Actions',
      accessor: (row) => (
        <div className="flex gap-2">
          {!row.isResolved && (
            <Button size="sm" variant="success" onClick={() => handleResolve(row)}>
              <FaCheckCircle className="w-4 h-4" />
            </Button>
          )}
          <Button size="sm" variant="danger" onClick={() => handleDelete(row.id)}>
            <FaTrash className="w-4 h-4" />
          </Button>
        </div>
      )
    }
  ];

  if (loading) {
    return (
      <MainLayout>
        <div className="flex justify-center items-center h-screen">
          <LoadingSpinner size="lg" />
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout>
      <PageHeader
        title="Alerts"
        subtitle="View and manage system alerts"
        icon={FaBell}
      />
      
      <div className="p-6 space-y-6">
        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
            {error}
          </div>
        )}

        {/* Stats Overview */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
          <StatCard
            title="Total Alerts"
            value={stats.total}
            icon={FaBell}
            color="blue"
          />
          <StatCard
            title="Unresolved"
            value={stats.unresolved}
            icon={FaExclamationTriangle}
            color={stats.unresolved > 0 ? 'red' : 'green'}
          />
          <StatCard
            title="Resolved"
            value={stats.resolved}
            icon={FaCheckCircle}
            color="green"
          />
          <StatCard
            title="Critical"
            value={stats.critical}
            icon={FaExclamationTriangle}
            color={stats.critical > 0 ? 'red' : 'gray'}
          />
        </div>

        {/* Filter */}
        <Card>
          <div className="p-4 flex gap-3">
            <Button 
              variant={filter === 'all' ? 'primary' : 'secondary'}
              size="sm"
              onClick={() => setFilter('all')}
            >
              All Alerts
            </Button>
            <Button 
              variant={filter === 'unresolved' ? 'primary' : 'secondary'}
              size="sm"
              onClick={() => setFilter('unresolved')}
            >
              Unresolved
            </Button>
            <Button 
              variant={filter === 'resolved' ? 'primary' : 'secondary'}
              size="sm"
              onClick={() => setFilter('resolved')}
            >
              Resolved
            </Button>
          </div>
        </Card>

        {/* Alerts Table */}
        <Card title={`${filter.charAt(0).toUpperCase() + filter.slice(1)} Alerts`}>
          {filteredAlerts.length === 0 ? (
            <div className="text-center py-12">
              <FaBell className="mx-auto h-12 w-12 text-gray-400 mb-4" />
              <h3 className="text-lg font-semibold text-gray-900 mb-2">No alerts found</h3>
              <p className="text-gray-600">
                {filter === 'unresolved' && 'Great! No unresolved alerts at the moment.'}
                {filter === 'resolved' && 'No resolved alerts yet.'}
                {filter === 'all' && 'No alerts in the system.'}
              </p>
            </div>
          ) : (
            <Table columns={columns} data={filteredAlerts} />
          )}
        </Card>
      </div>

      {/* Resolve Modal */}
      {showResolveModal && selectedAlert && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl max-w-md w-full">
            <div className="p-6 border-b border-gray-200">
              <h3 className="text-xl font-semibold text-gray-900">
                Resolve Alert
              </h3>
            </div>
            
            <form onSubmit={handleResolveSubmit} className="p-6 space-y-4">
              <div>
                <p className="text-sm text-gray-600 mb-2">
                  <strong>Room:</strong> {selectedAlert.roomNumber}
                </p>
                <p className="text-sm text-gray-600 mb-2">
                  <strong>Type:</strong> {selectedAlert.alertType}
                </p>
                <p className="text-sm text-gray-600 mb-4">
                  <strong>Message:</strong> {selectedAlert.message}
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Resolution Notes
                </label>
                <textarea
                  rows="4"
                  value={resolutionNotes}
                  onChange={(e) => setResolutionNotes(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Describe how this alert was resolved..."
                />
              </div>

              <div className="flex justify-end gap-3 pt-4">
                <Button type="button" variant="secondary" onClick={() => setShowResolveModal(false)}>
                  Cancel
                </Button>
                <Button type="submit" variant="success">
                  Mark as Resolved
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </MainLayout>
  );
};

export default Alerts;


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
  const [showDetailsModal, setShowDetailsModal] = useState(false);
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

  const handleViewDetails = (alert) => {
    setSelectedAlert(alert);
    setShowDetailsModal(true);
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
        <div className="flex gap-2" onClick={(e) => e.stopPropagation()}>
          {!row.isResolved && (
            <Button size="sm" variant="success" onClick={(e) => { e.stopPropagation(); handleResolve(row); }}>
              <FaCheckCircle className="w-4 h-4" />
            </Button>
          )}
          <Button size="sm" variant="danger" onClick={(e) => { e.stopPropagation(); handleDelete(row.id); }}>
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
        subtitle="View and manage system alerts. Click on an alert to view full details."
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
            <Table columns={columns} data={filteredAlerts} onRowClick={handleViewDetails} />
          )}
        </Card>
      </div>

      {/* Alert Details Modal */}
      {showDetailsModal && selectedAlert && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl max-w-3xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6 border-b border-gray-200 bg-gradient-to-r from-red-500 to-red-600">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  {selectedAlert.isResolved ? (
                    <FaCheckCircle className="text-white text-3xl" />
                  ) : (
                    <FaExclamationTriangle className="text-white text-3xl" />
                  )}
                  <div>
                    <h3 className="text-2xl font-bold text-white">
                      Alert Details
                    </h3>
                    <p className="text-red-100">
                      Room {selectedAlert.roomNumber} - {selectedAlert.alertType}
                    </p>
                  </div>
                </div>
                <button
                  onClick={() => setShowDetailsModal(false)}
                  className="text-white hover:text-gray-200 text-2xl"
                >
                  ×
                </button>
              </div>
            </div>
            
            <div className="p-6 space-y-6">
              {/* Status Banner */}
              <div className={`p-4 rounded-lg border-l-4 ${
                selectedAlert.isResolved
                  ? 'bg-green-50 border-green-500'
                  : 'bg-red-50 border-red-500'
              }`}>
                <div className="flex items-center justify-between">
                  <div>
                    <h4 className="font-semibold text-gray-900 mb-1">Alert Status</h4>
                    <p className="text-sm text-gray-600">
                      {selectedAlert.isResolved
                        ? `Resolved by ${selectedAlert.resolvedByUsername || 'Unknown'}`
                        : 'This alert is currently active and needs attention'
                      }
                    </p>
                  </div>
                  {selectedAlert.isResolved ? (
                    <Badge variant="success">
                      <FaCheckCircle className="inline mr-1" />
                      Resolved
                    </Badge>
                  ) : (
                    <Badge variant="error">
                      <FaExclamationTriangle className="inline mr-1" />
                      Active
                    </Badge>
                  )}
                </div>
              </div>

              {/* Alert Information Grid */}
              <div className="grid grid-cols-2 gap-6">
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-500 mb-1">Room</h4>
                  <p className="text-xl font-semibold text-gray-900">{selectedAlert.roomNumber}</p>
                </div>
                
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-500 mb-1">Alert Type</h4>
                  <p className="text-xl font-semibold text-gray-900">{selectedAlert.alertType}</p>
                </div>
                
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-500 mb-1">Severity</h4>
                  <Badge variant={getSeverityVariant(selectedAlert.severity)} className="text-lg">
                    {selectedAlert.severity}
                  </Badge>
                </div>
                
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-500 mb-1">Created</h4>
                  <p className="text-lg font-semibold text-gray-900">
                    {formatDateTime(selectedAlert.createdAt)}
                  </p>
                </div>
              </div>

              {/* Alert Message */}
              <div className="border-t pt-6">
                <h4 className="text-lg font-semibold text-gray-900 mb-3">Alert Message</h4>
                <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
                  <p className="text-gray-700">{selectedAlert.message}</p>
                </div>
              </div>

              {/* Resolution Information */}
              {selectedAlert.isResolved && (
                <div className="border-t pt-6">
                  <h4 className="text-lg font-semibold text-gray-900 mb-3">Resolution Details</h4>
                  <div className="space-y-4">
                    <div className="bg-green-50 border border-green-200 rounded-lg p-4">
                      <div className="grid grid-cols-2 gap-4 mb-3">
                        <div>
                          <p className="text-sm text-gray-600">Resolved By</p>
                          <p className="font-semibold text-gray-900">
                            {selectedAlert.resolvedByUsername || 'Unknown'}
                          </p>
                        </div>
                        <div>
                          <p className="text-sm text-gray-600">Resolved At</p>
                          <p className="font-semibold text-gray-900">
                            {selectedAlert.resolvedAt ? formatDateTime(selectedAlert.resolvedAt) : '-'}
                          </p>
                        </div>
                      </div>
                      {selectedAlert.resolutionNotes && (
                        <div className="border-t border-green-300 pt-3 mt-3">
                          <p className="text-sm text-gray-600 mb-2">Resolution Notes:</p>
                          <p className="text-gray-800 whitespace-pre-wrap">{selectedAlert.resolutionNotes}</p>
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              )}

              {/* Additional Information */}
              <div className="border-t pt-6">
                <h4 className="text-lg font-semibold text-gray-900 mb-3">Additional Information</h4>
                <div className="space-y-2 text-sm">
                  <div className="flex justify-between">
                    <span className="text-gray-600">Alert ID:</span>
                    <span className="font-medium text-gray-900">#{selectedAlert.id}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Room ID:</span>
                    <span className="font-medium text-gray-900">#{selectedAlert.roomId}</span>
                  </div>
                </div>
              </div>

              {/* Action Buttons */}
              <div className="flex gap-3 pt-4 border-t">
                {!selectedAlert.isResolved && (
                  <Button 
                    variant="success" 
                    onClick={() => {
                      setShowDetailsModal(false);
                      handleResolve(selectedAlert);
                    }}
                    className="flex-1"
                  >
                    <FaCheckCircle className="inline mr-2" />
                    Resolve Alert
                  </Button>
                )}
                <Button 
                  variant="danger" 
                  onClick={() => {
                    setShowDetailsModal(false);
                    handleDelete(selectedAlert.id);
                  }}
                  className="flex-1"
                >
                  <FaTrash className="inline mr-2" />
                  Delete Alert
                </Button>
                <Button 
                  variant="secondary" 
                  onClick={() => setShowDetailsModal(false)}
                  className="flex-1"
                >
                  Close
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

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


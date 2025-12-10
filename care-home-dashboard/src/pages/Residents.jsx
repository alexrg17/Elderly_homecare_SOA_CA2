import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext.jsx';
import MainLayout from '../components/layout/MainLayout.jsx';
import PageHeader from '../components/common/PageHeader.jsx';
import Card from '../components/common/Card.jsx';
import Button from '../components/common/Button.jsx';
import Table from '../components/common/Table.jsx';
import Badge from '../components/common/Badge.jsx';
import LoadingSpinner from '../components/common/LoadingSpinner.jsx';
import { FaPlus, FaEdit, FaTrash, FaUser } from 'react-icons/fa';
import residentsService from '../services/residentsService';
import roomsService from '../services/roomsService';

const Residents = () => {
  const { user } = useAuth();
  const isAdmin = user?.role === 'Admin';
  const [residents, setResidents] = useState([]);
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [selectedResident, setSelectedResident] = useState(null);
  const [editingResident, setEditingResident] = useState(null);
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    medicalConditions: '',
    emergencyContact: '',
    emergencyPhone: '',
    roomId: ''
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [residentsData, roomsData] = await Promise.all([
        residentsService.getAll(),
        roomsService.getAll()
      ]);
      setResidents(residentsData);
      setRooms(roomsData);
      setError(null);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to fetch residents');
      console.error('Error fetching data:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleAdd = () => {
    setEditingResident(null);
    setFormData({
      firstName: '',
      lastName: '',
      dateOfBirth: '',
      medicalConditions: '',
      emergencyContact: '',
      emergencyPhone: '',
      roomId: ''
    });
    setShowModal(true);
  };

  const handleViewDetails = (resident) => {
    setSelectedResident(resident);
    setShowDetailsModal(true);
  };

  const handleEdit = (resident) => {
    setEditingResident(resident);
    setFormData({
      firstName: resident.firstName,
      lastName: resident.lastName,
      dateOfBirth: resident.dateOfBirth.split('T')[0],
      medicalConditions: resident.medicalConditions || '',
      emergencyContact: resident.emergencyContact || '',
      emergencyPhone: resident.emergencyPhone || '',
      roomId: resident.roomId || ''
    });
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this resident?')) return;
    
    try {
      await residentsService.delete(id);
      fetchData();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to delete resident');
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    try {
      const submitData = {
        ...formData,
        roomId: formData.roomId ? parseInt(formData.roomId) : null
      };

      if (editingResident) {
        await residentsService.update(editingResident.id, submitData);
      } else {
        await residentsService.create(submitData);
      }
      
      setShowModal(false);
      fetchData();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to save resident');
    }
  };

  const columns = [
    { header: 'Name', accessor: (row) => `${row.firstName} ${row.lastName}` },
    { header: 'Age', accessor: 'age' },
    { header: 'Room', accessor: (row) => row.roomNumber || 'Unassigned' },
    { 
      header: 'Date of Birth', 
      accessor: (row) => new Date(row.dateOfBirth).toLocaleDateString()
    },
    {
      header: 'Status',
      accessor: (row) => (
        <Badge variant={row.isActive ? 'success' : 'error'}>
          {row.isActive ? 'Active' : 'Inactive'}
        </Badge>
      )
    },
    {
      header: 'Actions',
      accessor: (row) => (
        <div className="flex gap-2" onClick={(e) => e.stopPropagation()}>
          <Button size="sm" variant="secondary" onClick={(e) => { e.stopPropagation(); handleEdit(row); }}>
            <FaEdit className="w-4 h-4" />
          </Button>
          {isAdmin && (
            <Button size="sm" variant="danger" onClick={(e) => { e.stopPropagation(); handleDelete(row.id); }}>
              <FaTrash className="w-4 h-4" />
            </Button>
          )}
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
        title="Residents"
        subtitle="Manage elderly care home residents. Click on a resident to view full details."
        icon={FaUser}
        actions={
          <Button icon={FaPlus} onClick={handleAdd}>
            Add Resident
          </Button>
        }
      />
      
      <div className="p-6">
        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
            {error}
          </div>
        )}

        <Card>
          {residents.length === 0 ? (
            <div className="text-center py-12">
              <FaUser className="mx-auto h-12 w-12 text-gray-400 mb-4" />
              <h3 className="text-lg font-semibold text-gray-900 mb-2">No residents found</h3>
              <p className="text-gray-600 mb-4">Get started by adding your first resident.</p>
              <Button icon={FaPlus} onClick={handleAdd}>
                Add Resident
              </Button>
            </div>
          ) : (
            <Table columns={columns} data={residents} onRowClick={handleViewDetails} />
          )}
        </Card>
      </div>

      {/* Resident Details Modal */}
      {showDetailsModal && selectedResident && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6 border-b border-gray-200 bg-gradient-to-r from-blue-500 to-blue-600">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <FaUser className="text-white text-3xl" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">
                      {selectedResident.firstName} {selectedResident.lastName}
                    </h3>
                    <p className="text-blue-100">
                      {selectedResident.age} years old
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
                selectedResident.isActive
                  ? 'bg-green-50 border-green-500'
                  : 'bg-gray-50 border-gray-500'
              }`}>
                <div className="flex items-center justify-between">
                  <div>
                    <h4 className="font-semibold text-gray-900 mb-1">Resident Status</h4>
                    <p className="text-sm text-gray-600">
                      {selectedResident.isActive ? 'Currently active in the care home' : 'Inactive'}
                    </p>
                  </div>
                  <Badge variant={selectedResident.isActive ? 'success' : 'error'}>
                    {selectedResident.isActive ? 'Active' : 'Inactive'}
                  </Badge>
                </div>
              </div>

              {/* Personal Information Grid */}
              <div className="grid grid-cols-2 gap-6">
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-500 mb-1">Date of Birth</h4>
                  <p className="text-lg font-semibold text-gray-900">
                    {new Date(selectedResident.dateOfBirth).toLocaleDateString('en-US', {
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric'
                    })}
                  </p>
                </div>
                
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-500 mb-1">Age</h4>
                  <p className="text-lg font-semibold text-gray-900">
                    {selectedResident.age} years
                  </p>
                </div>
                
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-500 mb-1">Room Assignment</h4>
                  <p className="text-lg font-semibold text-gray-900">
                    {selectedResident.roomNumber || 'Unassigned'}
                  </p>
                </div>
                
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-500 mb-1">Admission Date</h4>
                  <p className="text-lg font-semibold text-gray-900">
                    {new Date(selectedResident.admissionDate).toLocaleDateString()}
                  </p>
                </div>
              </div>

              {/* Medical Conditions Section */}
              {selectedResident.medicalConditions && (
                <div className="border-t pt-6">
                  <h4 className="text-lg font-semibold text-gray-900 mb-3">Medical Conditions</h4>
                  <div className="bg-red-50 border border-red-200 rounded-lg p-4">
                    <p className="text-gray-700 whitespace-pre-wrap">{selectedResident.medicalConditions}</p>
                  </div>
                </div>
              )}

              {/* Emergency Contact Section */}
              {(selectedResident.emergencyContact || selectedResident.emergencyPhone) && (
                <div className="border-t pt-6">
                  <h4 className="text-lg font-semibold text-gray-900 mb-3">Emergency Contact</h4>
                  <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                    <div className="space-y-2">
                      {selectedResident.emergencyContact && (
                        <div>
                          <p className="text-sm text-gray-600">Name</p>
                          <p className="text-lg font-semibold text-gray-900">{selectedResident.emergencyContact}</p>
                        </div>
                      )}
                      {selectedResident.emergencyPhone && (
                        <div>
                          <p className="text-sm text-gray-600">Phone</p>
                          <p className="text-lg font-semibold text-gray-900">{selectedResident.emergencyPhone}</p>
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
                    <span className="text-gray-600">Resident ID:</span>
                    <span className="font-medium text-gray-900">#{selectedResident.id}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Full Name:</span>
                    <span className="font-medium text-gray-900">
                      {selectedResident.firstName} {selectedResident.lastName}
                    </span>
                  </div>
                </div>
              </div>

              {/* Action Buttons */}
              <div className="flex gap-3 pt-4 border-t">
                <Button 
                  variant="secondary" 
                  onClick={() => {
                    setShowDetailsModal(false);
                    handleEdit(selectedResident);
                  }}
                  className="flex-1"
                >
                  <FaEdit className="inline mr-2" />
                  Edit Resident
                </Button>
                {isAdmin && (
                  <Button 
                    variant="danger" 
                    onClick={() => {
                      setShowDetailsModal(false);
                      handleDelete(selectedResident.id);
                    }}
                    className="flex-1"
                  >
                    <FaTrash className="inline mr-2" />
                    Delete Resident
                  </Button>
                )}
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Edit/Add Resident Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6 border-b border-gray-200">
              <h3 className="text-xl font-semibold text-gray-900">
                {editingResident ? 'Edit Resident' : 'Add New Resident'}
              </h3>
            </div>
            
            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    First Name *
                  </label>
                  <input
                    type="text"
                    required
                    value={formData.firstName}
                    onChange={(e) => setFormData({...formData, firstName: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Last Name *
                  </label>
                  <input
                    type="text"
                    required
                    value={formData.lastName}
                    onChange={(e) => setFormData({...formData, lastName: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Date of Birth *
                  </label>
                  <input
                    type="date"
                    required
                    value={formData.dateOfBirth}
                    onChange={(e) => setFormData({...formData, dateOfBirth: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Room
                  </label>
                  <select
                    value={formData.roomId}
                    onChange={(e) => setFormData({...formData, roomId: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  >
                    <option value="">Unassigned</option>
                    {rooms.map(room => {
                      const isFull = room.residentCount >= room.capacity;
                      const isCurrentRoom = editingResident && editingResident.roomId === room.id;
                      // Allow selection if it's the current room OR if the room is not full
                      const isDisabled = isFull && !isCurrentRoom;
                      
                      return (
                        <option 
                          key={room.id} 
                          value={room.id}
                          disabled={isDisabled}
                        >
                          {room.roomNumber} - {room.roomName || 'No name'} ({room.residentCount}/{room.capacity})
                          {isFull && !isCurrentRoom ? ' - FULL' : ''}
                        </option>
                      );
                    })}
                  </select>
                  <p className="text-xs text-gray-500 mt-1">
                    Showing occupancy (current/capacity). Full rooms are disabled.
                  </p>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Medical Conditions
                </label>
                <textarea
                  rows="3"
                  value={formData.medicalConditions}
                  onChange={(e) => setFormData({...formData, medicalConditions: e.target.value})}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="e.g., Diabetes, Hypertension..."
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Emergency Contact
                  </label>
                  <input
                    type="text"
                    value={formData.emergencyContact}
                    onChange={(e) => setFormData({...formData, emergencyContact: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Emergency Phone
                  </label>
                  <input
                    type="tel"
                    value={formData.emergencyPhone}
                    onChange={(e) => setFormData({...formData, emergencyPhone: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
              </div>

              <div className="flex justify-end gap-3 pt-4">
                <Button type="button" variant="secondary" onClick={() => setShowModal(false)}>
                  Cancel
                </Button>
                <Button type="submit">
                  {editingResident ? 'Update' : 'Create'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </MainLayout>
  );
};

export default Residents;


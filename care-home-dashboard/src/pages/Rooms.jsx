import { useState, useEffect } from 'react';
import MainLayout from '../components/layout/MainLayout.jsx';
import PageHeader from '../components/common/PageHeader.jsx';
import Card from '../components/common/Card.jsx';
import Button from '../components/common/Button.jsx';
import Table from '../components/common/Table.jsx';
import Badge from '../components/common/Badge.jsx';
import LoadingSpinner from '../components/common/LoadingSpinner.jsx';
import { FaPlus, FaEdit, FaTrash, FaDoorOpen, FaExclamationTriangle, FaCheckCircle, FaTimesCircle, FaUsers } from 'react-icons/fa';
import roomsService from '../services/roomsService';

const Rooms = () => {
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [selectedRoom, setSelectedRoom] = useState(null);
  const [editingRoom, setEditingRoom] = useState(null);
  const [formData, setFormData] = useState({
    roomNumber: '',
    roomName: '',
    floor: '',
    capacity: 1,
    notes: ''
  });

  // Helper function to format floor numbers (1 -> 1st Floor, 2 -> 2nd Floor, etc.)
  const formatFloor = (floor) => {
    const num = parseInt(floor);
    if (isNaN(num)) return floor;
    
    const suffix = (num) => {
      const j = num % 10;
      const k = num % 100;
      if (j === 1 && k !== 11) return 'st';
      if (j === 2 && k !== 12) return 'nd';
      if (j === 3 && k !== 13) return 'rd';
      return 'th';
    };
    
    return `${num}${suffix(num)} Floor`;
  };

  useEffect(() => {
    fetchRooms();
  }, []);

  const fetchRooms = async () => {
    try {
      setLoading(true);
      const data = await roomsService.getAll();
      setRooms(data);
      setError(null);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to fetch rooms');
      console.error('Error fetching rooms:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleAdd = () => {
    setEditingRoom(null);
    setFormData({
      roomNumber: '',
      roomName: '',
      floor: '',
      capacity: 1,
      notes: ''
    });
    setShowModal(true);
  };

  const handleViewDetails = (room) => {
    setSelectedRoom(room);
    setShowDetailsModal(true);
  };

  const handleEdit = (room) => {
    setEditingRoom(room);
    setFormData({
      roomNumber: room.roomNumber,
      roomName: room.roomName || '',
      floor: room.floor,
      capacity: room.capacity,
      notes: room.notes || ''
    });
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this room?')) return;
    
    try {
      await roomsService.delete(id);
      fetchRooms();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to delete room');
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    try {
      const submitData = {
        ...formData,
        capacity: parseInt(formData.capacity)
      };

      if (editingRoom) {
        await roomsService.update(editingRoom.id, submitData);
      } else {
        await roomsService.create(submitData);
      }
      
      setShowModal(false);
      fetchRooms();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to save room');
    }
  };

  const columns = [
    { header: 'Room Number', accessor: 'roomNumber' },
    { header: 'Room Name', accessor: (row) => row.roomName || '-' },
    { header: 'Floor', accessor: (row) => formatFloor(row.floor) },
    { 
      header: 'Capacity', 
      accessor: (row) => `${row.residentCount}/${row.capacity}` 
    },
    {
      header: 'Status',
      accessor: (row) => {
        const isFull = row.residentCount >= row.capacity;
        const isEmpty = row.residentCount === 0;
        
        if (isFull) {
          return (
            <Badge variant="error">
              <FaTimesCircle className="inline mr-1" />
              Full
            </Badge>
          );
        } else if (isEmpty) {
          return (
            <Badge variant="success">
              <FaCheckCircle className="inline mr-1" />
              Available
            </Badge>
          );
        } else {
          return (
            <Badge variant="success">
              <FaUsers className="inline mr-1" />
              Available
            </Badge>
          );
        }
      }
    },
    {
      header: 'Alerts',
      accessor: (row) => (
        row.activeAlertCount > 0 ? (
          <div className="flex items-center gap-1 text-red-600">
            <FaExclamationTriangle />
            <span className="font-semibold">{row.activeAlertCount}</span>
          </div>
        ) : (
          <span className="text-gray-400">None</span>
        )
      )
    },
    {
      header: 'Sensor Readings',
      accessor: 'sensorReadingCount'
    },
    {
      header: 'Actions',
      accessor: (row) => (
        <div className="flex gap-2" onClick={(e) => e.stopPropagation()}>
          <Button size="sm" variant="secondary" onClick={(e) => { e.stopPropagation(); handleEdit(row); }}>
            <FaEdit className="w-4 h-4" />
          </Button>
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
        title="Rooms"
        subtitle="Manage care home rooms and occupancy. Click on a room to view full details."
        icon={FaDoorOpen}
        actions={
          <Button icon={FaPlus} onClick={handleAdd}>
            Add Room
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
          {rooms.length === 0 ? (
            <div className="text-center py-12">
              <FaDoorOpen className="mx-auto h-12 w-12 text-gray-400 mb-4" />
              <h3 className="text-lg font-semibold text-gray-900 mb-2">No rooms found</h3>
              <p className="text-gray-600 mb-4">Get started by adding your first room.</p>
              <Button icon={FaPlus} onClick={handleAdd}>
                Add Room
              </Button>
            </div>
          ) : (
            <Table columns={columns} data={rooms} onRowClick={handleViewDetails} />
          )}
        </Card>
      </div>

      {/* Room Details Modal */}
      {showDetailsModal && selectedRoom && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6 border-b border-gray-200 bg-gradient-to-r from-blue-500 to-blue-600">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <FaDoorOpen className="text-white text-3xl" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">
                      Room {selectedRoom.roomNumber}
                    </h3>
                    <p className="text-blue-100">
                      {selectedRoom.roomName || 'No name assigned'}
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
                selectedRoom.residentCount >= selectedRoom.capacity
                  ? 'bg-red-50 border-red-500'
                  : selectedRoom.residentCount === 0
                  ? 'bg-green-50 border-green-500'
                  : 'bg-blue-50 border-blue-500'
              }`}>
                <div className="flex items-center justify-between">
                  <div>
                    <h4 className="font-semibold text-gray-900 mb-1">Room Status</h4>
                    <p className="text-sm text-gray-600">
                      {selectedRoom.residentCount >= selectedRoom.capacity
                        ? 'This room is at full capacity'
                        : selectedRoom.residentCount === 0
                        ? 'This room is currently available'
                        : `This room has ${selectedRoom.capacity - selectedRoom.residentCount} bed(s) available`
                      }
                    </p>
                  </div>
                  {selectedRoom.residentCount >= selectedRoom.capacity ? (
                    <Badge variant="error">
                      <FaTimesCircle className="inline mr-1" />
                      Full
                    </Badge>
                  ) : (
                    <Badge variant="success">
                      <FaCheckCircle className="inline mr-1" />
                      Available
                    </Badge>
                  )}
                </div>
              </div>

              {/* Room Information Grid */}
              <div className="grid grid-cols-2 gap-6">
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-500 mb-1">Floor</h4>
                  <p className="text-xl font-semibold text-gray-900">{formatFloor(selectedRoom.floor)}</p>
                </div>
                
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-500 mb-1">Capacity</h4>
                  <p className="text-xl font-semibold text-gray-900">
                    {selectedRoom.residentCount} / {selectedRoom.capacity}
                  </p>
                </div>
                
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-500 mb-1">Active Alerts</h4>
                  <p className={`text-xl font-semibold ${selectedRoom.activeAlertCount > 0 ? 'text-red-600' : 'text-green-600'}`}>
                    {selectedRoom.activeAlertCount || 0}
                  </p>
                </div>
                
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-500 mb-1">Sensor Readings</h4>
                  <p className="text-xl font-semibold text-gray-900">
                    {selectedRoom.sensorReadingCount || 0}
                  </p>
                </div>
              </div>

              {/* Notes Section */}
              {selectedRoom.notes && (
                <div className="border-t pt-6">
                  <h4 className="text-lg font-semibold text-gray-900 mb-3">Notes</h4>
                  <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
                    <p className="text-gray-700 whitespace-pre-wrap">{selectedRoom.notes}</p>
                  </div>
                </div>
              )}

              {/* Additional Information */}
              <div className="border-t pt-6">
                <h4 className="text-lg font-semibold text-gray-900 mb-3">Additional Information</h4>
                <div className="space-y-2 text-sm">
                  <div className="flex justify-between">
                    <span className="text-gray-600">Room ID:</span>
                    <span className="font-medium text-gray-900">#{selectedRoom.id}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Created:</span>
                    <span className="font-medium text-gray-900">
                      {new Date(selectedRoom.createdAt).toLocaleDateString()}
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
                    handleEdit(selectedRoom);
                  }}
                  className="flex-1"
                >
                  <FaEdit className="inline mr-2" />
                  Edit Room
                </Button>
                <Button 
                  variant="danger" 
                  onClick={() => {
                    setShowDetailsModal(false);
                    handleDelete(selectedRoom.id);
                  }}
                  className="flex-1"
                >
                  <FaTrash className="inline mr-2" />
                  Delete Room
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Edit/Add Room Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl max-w-md w-full">
            <div className="p-6 border-b border-gray-200">
              <h3 className="text-xl font-semibold text-gray-900">
                {editingRoom ? 'Edit Room' : 'Add New Room'}
              </h3>
            </div>
            
            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Room Number *
                </label>
                <input
                  type="text"
                  required
                  value={formData.roomNumber}
                  onChange={(e) => setFormData({...formData, roomNumber: e.target.value})}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="e.g., 101"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Room Name
                </label>
                <input
                  type="text"
                  value={formData.roomName}
                  onChange={(e) => setFormData({...formData, roomName: e.target.value})}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="e.g., Sunrise Suite"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Floor *
                  </label>
                  <input
                    type="text"
                    required
                    value={formData.floor}
                    onChange={(e) => setFormData({...formData, floor: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    placeholder="e.g., 1"
                  />
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Capacity *
                  </label>
                  <input
                    type="number"
                    required
                    min="1"
                    value={formData.capacity}
                    onChange={(e) => setFormData({...formData, capacity: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Notes
                </label>
                <textarea
                  rows="3"
                  value={formData.notes}
                  onChange={(e) => setFormData({...formData, notes: e.target.value})}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Any additional notes about this room..."
                />
              </div>

              <div className="flex justify-end gap-3 pt-4">
                <Button type="button" variant="secondary" onClick={() => setShowModal(false)}>
                  Cancel
                </Button>
                <Button type="submit">
                  {editingRoom ? 'Update' : 'Create'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </MainLayout>
  );
};

export default Rooms;


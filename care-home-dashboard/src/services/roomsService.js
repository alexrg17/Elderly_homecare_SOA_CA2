import apiClient from './apiClient';

const roomsService = {
  // Get all rooms
  getAll: async () => {
    const response = await apiClient.get('/rooms');
    return response.data;
  },

  // Get room by ID
  getById: async (id) => {
    const response = await apiClient.get(`/rooms/${id}`);
    return response.data;
  },

  // Create new room
  create: async (roomData) => {
    const response = await apiClient.post('/rooms', roomData);
    return response.data;
  },

  // Update room
  update: async (id, roomData) => {
    const response = await apiClient.put(`/rooms/${id}`, roomData);
    return response.data;
  },

  // Delete room
  delete: async (id) => {
    const response = await apiClient.delete(`/rooms/${id}`);
    return response.data;
  },

  // Get available rooms
  getAvailable: async () => {
    const response = await apiClient.get('/rooms/available');
    return response.data;
  }
};

export default roomsService;


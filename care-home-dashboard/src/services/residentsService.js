import apiClient from './apiClient';

const residentsService = {
  // Get all residents
  getAll: async () => {
    const response = await apiClient.get('/residents');
    return response.data;
  },

  // Get resident by ID
  getById: async (id) => {
    const response = await apiClient.get(`/residents/${id}`);
    return response.data;
  },

  // Create new resident
  create: async (residentData) => {
    const response = await apiClient.post('/residents', residentData);
    return response.data;
  },

  // Update resident
  update: async (id, residentData) => {
    const response = await apiClient.put(`/residents/${id}`, residentData);
    return response.data;
  },

  // Delete resident
  delete: async (id) => {
    const response = await apiClient.delete(`/residents/${id}`);
    return response.data;
  },

  // Get residents by room
  getByRoom: async (roomId) => {
    const response = await apiClient.get(`/residents/room/${roomId}`);
    return response.data;
  }
};

export default residentsService;


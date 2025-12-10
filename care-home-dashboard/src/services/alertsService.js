import apiClient from './apiClient';

const alertsService = {
  // Get all alerts
  getAll: async () => {
    const response = await apiClient.get('/alerts');
    return response.data;
  },

  // Get unresolved alerts
  getUnresolved: async () => {
    const response = await apiClient.get('/alerts/unresolved');
    return response.data;
  },

  // Get alert by ID
  getById: async (id) => {
    const response = await apiClient.get(`/alerts/${id}`);
    return response.data;
  },

  // Get alerts by room
  getByRoom: async (roomId) => {
    const response = await apiClient.get(`/alerts/room/${roomId}`);
    return response.data;
  },

  // Create new alert
  create: async (alertData) => {
    const response = await apiClient.post('/alerts', alertData);
    return response.data;
  },

  // Update alert
  update: async (id, alertData) => {
    const response = await apiClient.put(`/alerts/${id}`, alertData);
    return response.data;
  },

  // Resolve alert
  resolve: async (id, userId, resolutionNotes) => {
    const response = await apiClient.post(`/alerts/${id}/resolve`, {
      userId,
      resolutionNotes
    });
    return response.data;
  },

  // Delete alert
  delete: async (id) => {
    const response = await apiClient.delete(`/alerts/${id}`);
    return response.data;
  }
};

export default alertsService;


import apiClient from './apiClient';

const alertsService = {
  getAll: async () => {
    const response = await apiClient.get('/alerts');
    return response.data;
  },

  getById: async (id) => {
    const response = await apiClient.get(`/alerts/${id}`);
    return response.data;
  },

  getBySeverity: async (severity) => {
    const response = await apiClient.get(`/alerts/severity/${severity}`);
    return response.data;
  },

  getByRoom: async (roomId) => {
    const response = await apiClient.get(`/alerts/room/${roomId}`);
    return response.data;
  },

  create: async (alertData) => {
    const response = await apiClient.post('/alerts', alertData);
    return response.data;
  },

  update: async (id, alertData) => {
    const response = await apiClient.put(`/alerts/${id}`, alertData);
    return response.data;
  },

  delete: async (id) => {
    const response = await apiClient.delete(`/alerts/${id}`);
    return response.data;
  },
};

export default alertsService;


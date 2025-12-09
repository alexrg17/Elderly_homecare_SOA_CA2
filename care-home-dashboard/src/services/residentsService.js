import apiClient from './apiClient';

const residentsService = {
  getAll: async () => {
    const response = await apiClient.get('/residents');
    return response.data;
  },

  getById: async (id) => {
    const response = await apiClient.get(`/residents/${id}`);
    return response.data;
  },

  getActive: async () => {
    const response = await apiClient.get('/residents/active');
    return response.data;
  },

  create: async (residentData) => {
    const response = await apiClient.post('/residents', residentData);
    return response.data;
  },

  update: async (id, residentData) => {
    const response = await apiClient.put(`/residents/${id}`, residentData);
    return response.data;
  },

  delete: async (id) => {
    const response = await apiClient.delete(`/residents/${id}`);
    return response.data;
  },
};

export default residentsService;


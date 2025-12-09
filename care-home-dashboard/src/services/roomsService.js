import apiClient from './apiClient';

const roomsService = {
  getAll: async () => {
    const response = await apiClient.get('/rooms');
    return response.data;
  },

  getById: async (id) => {
    const response = await apiClient.get(`/rooms/${id}`);
    return response.data;
  },

  create: async (roomData) => {
    const response = await apiClient.post('/rooms', roomData);
    return response.data;
  },

  update: async (id, roomData) => {
    const response = await apiClient.put(`/rooms/${id}`, roomData);
    return response.data;
  },

  delete: async (id) => {
    const response = await apiClient.delete(`/rooms/${id}`);
    return response.data;
  },
};

export default roomsService;


import apiClient from './apiClient';

const sensorsService = {
  getAll: async () => {
    const response = await apiClient.get('/sensordata');
    return response.data;
  },

  getById: async (id) => {
    const response = await apiClient.get(`/sensordata/${id}`);
    return response.data;
  },

  getByRoom: async (roomId) => {
    const response = await apiClient.get(`/sensordata/room/${roomId}`);
    return response.data;
  },

  getLatestByRoom: async (roomId) => {
    const response = await apiClient.get(`/sensordata/room/${roomId}/latest`);
    return response.data;
  },

  getRecent: async (count = 50) => {
    const response = await apiClient.get(`/sensordata/recent?count=${count}`);
    return response.data;
  },

  create: async (sensorData) => {
    const response = await apiClient.post('/sensordata', sensorData);
    return response.data;
  },
};

export default sensorsService;


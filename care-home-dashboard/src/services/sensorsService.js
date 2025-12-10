import apiClient from './apiClient';

const sensorsService = {
  // Get all sensor readings
  getAll: async () => {
    const response = await apiClient.get('/sensordata');
    return response.data;
  },

  // Get recent sensor readings
  getRecent: async (count = 50) => {
    const response = await apiClient.get(`/sensordata/recent?count=${count}`);
    return response.data;
  },

  // Get sensor reading by ID
  getById: async (id) => {
    const response = await apiClient.get(`/sensordata/${id}`);
    return response.data;
  },

  // Get sensor readings by room
  getByRoom: async (roomId) => {
    const response = await apiClient.get(`/sensordata/room/${roomId}`);
    return response.data;
  },

  // Get sensor readings by date range
  getByDateRange: async (startDate, endDate) => {
    const response = await apiClient.get('/sensordata/daterange', {
      params: { startDate, endDate }
    });
    return response.data;
  },

  // Create new sensor reading
  create: async (sensorData) => {
    const response = await apiClient.post('/sensordata', sensorData);
    return response.data;
  },

  // Update sensor reading
  update: async (id, sensorData) => {
    const response = await apiClient.put(`/sensordata/${id}`, sensorData);
    return response.data;
  },

  // Delete sensor reading
  delete: async (id) => {
    const response = await apiClient.delete(`/sensordata/${id}`);
    return response.data;
  }
};

export default sensorsService;


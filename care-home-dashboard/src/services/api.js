import { 
  authService, 
  residentsService, 
  roomsService, 
  sensorsService, 
  alertsService,
  usersService 
} from './index';

const api = {
  // Auth
  login: authService.login,
  logout: authService.logout,
  register: usersService.register,
  getCurrentUser: authService.getCurrentUser,
  getToken: authService.getToken,
  isAuthenticated: authService.isAuthenticated,

  // Users
  getUsers: usersService.getAll,
  getUserById: usersService.getById,
  updateUser: usersService.update,
  deleteUser: usersService.delete,
  resetPassword: usersService.resetPassword,

  // Residents
  getResidents: residentsService.getAll,
  getResidentById: residentsService.getById,
  createResident: residentsService.create,
  updateResident: residentsService.update,
  deleteResident: residentsService.delete,

  // Rooms
  getRooms: roomsService.getAll,
  getRoomById: roomsService.getById,
  createRoom: roomsService.create,
  updateRoom: roomsService.update,
  deleteRoom: roomsService.delete,

  // Sensors
  getSensorData: sensorsService.getAll,
  getSensorDataById: sensorsService.getById,
  getRecentSensorData: sensorsService.getRecent,
  createSensorData: sensorsService.create,

  // Alerts
  getAlerts: alertsService.getAll,
  getAlertById: alertsService.getById,
  resolveAlert: alertsService.resolve,
  deleteAlert: alertsService.delete,
};

export default api;


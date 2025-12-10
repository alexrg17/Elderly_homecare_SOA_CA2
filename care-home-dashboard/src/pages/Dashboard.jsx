import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext.jsx';
import { FaUsers, FaDoorOpen, FaThermometerHalf, FaBell, FaCheckCircle, FaExclamationTriangle, FaChartLine } from 'react-icons/fa';
import MainLayout from '../components/layout/MainLayout.jsx';
import PageHeader from '../components/common/PageHeader.jsx';
import StatCard from '../components/common/StatCard.jsx';
import Card from '../components/common/Card.jsx';
import LoadingSpinner from '../components/common/LoadingSpinner.jsx';
import residentsService from '../services/residentsService';
import roomsService from '../services/roomsService';
import sensorsService from '../services/sensorsService';
import alertsService from '../services/alertsService';

const Dashboard = () => {
  const { user } = useAuth();
  const [stats, setStats] = useState({
    totalResidents: 0,
    activeResidents: 0,
    totalRooms: 0,
    availableRooms: 0,
    occupancyRate: 0,
    totalSensors: 0,
    recentSensors: 0,
    totalAlerts: 0,
    unresolvedAlerts: 0,
    criticalAlerts: 0
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      const [residents, rooms, sensors, alerts] = await Promise.all([
        residentsService.getAll(),
        roomsService.getAll(),
        sensorsService.getRecent(50),
        alertsService.getAll()
      ]);

      const totalCapacity = rooms.reduce((sum, room) => sum + room.capacity, 0);
      const occupiedBeds = rooms.reduce((sum, room) => sum + room.residentCount, 0);
      const occupancyRate = totalCapacity > 0 ? Math.round((occupiedBeds / totalCapacity) * 100) : 0;

      setStats({
        totalResidents: residents.length,
        activeResidents: residents.filter(r => r.isActive).length,
        totalRooms: rooms.length,
        availableRooms: rooms.filter(r => r.residentCount < r.capacity).length,
        occupancyRate: occupancyRate,
        totalSensors: sensors.length,
        recentSensors: sensors.filter(s => {
          const sensorDate = new Date(s.timestamp);
          const hourAgo = new Date(Date.now() - 60 * 60 * 1000);
          return sensorDate > hourAgo;
        }).length,
        totalAlerts: alerts.length,
        unresolvedAlerts: alerts.filter(a => !a.isResolved).length,
        criticalAlerts: alerts.filter(a => !a.isResolved && a.severity.toLowerCase() === 'critical').length
      });
      setError(null);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to fetch dashboard data');
      console.error('Error fetching dashboard data:', err);
    } finally {
      setLoading(false);
    }
  };

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
        title="Dashboard"
        subtitle={`Welcome back, ${user?.fullName || user?.username}!`}
      />
      
      <div className="p-6 space-y-6">
        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
            {error}
          </div>
        )}

        {/* Critical Alerts Banner */}
        {stats.criticalAlerts > 0 && (
          <div className="bg-red-100 border-l-4 border-red-600 p-4 rounded-r-lg shadow-md">
            <div className="flex items-center gap-3">
              <FaExclamationTriangle className="text-red-600 text-3xl flex-shrink-0" />
              <div className="flex-1">
                <h3 className="font-bold text-red-900 text-lg">
                  {stats.criticalAlerts} Critical Alert{stats.criticalAlerts !== 1 ? 's' : ''} Require Immediate Attention
                </h3>
                <p className="text-sm text-red-800 mt-1">
                  Please navigate to the Alerts page to review and resolve critical issues.
                </p>
              </div>
              <a
                href="/alerts"
                className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors font-medium"
              >
                View Alerts
              </a>
            </div>
          </div>
        )}

        {/* Main Stats Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <StatCard
            title="Active Residents"
            value={stats.activeResidents}
            icon={FaUsers}
            color="blue"
            subtitle={`${stats.totalResidents} total residents`}
          />
          <StatCard
            title="Available Rooms"
            value={stats.availableRooms}
            icon={FaDoorOpen}
            color="green"
            subtitle={`${stats.totalRooms} total rooms`}
          />
          <StatCard
            title="Recent Sensors"
            value={stats.recentSensors}
            icon={FaThermometerHalf}
            color="cyan"
            subtitle="in last hour"
          />
          <StatCard
            title="Active Alerts"
            value={stats.unresolvedAlerts}
            icon={FaBell}
            color={stats.unresolvedAlerts > 0 ? 'red' : 'green'}
            subtitle={stats.unresolvedAlerts > 0 ? 'need attention' : 'all clear'}
          />
        </div>

        {/* Key Metrics */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {/* Occupancy Overview */}
          <Card>
            <div className="p-6">
              <div className="flex items-center justify-between mb-4">
                <h3 className="text-lg font-semibold text-gray-900">Room Occupancy</h3>
                <FaChartLine className="text-blue-500 text-2xl" />
              </div>
              <div className="space-y-4">
                <div>
                  <div className="flex justify-between items-center mb-2">
                    <span className="text-3xl font-bold text-blue-600">{stats.occupancyRate}%</span>
                    <span className="text-sm text-gray-600">Occupancy Rate</span>
                  </div>
                  <div className="w-full bg-gray-200 rounded-full h-3">
                    <div
                      className="bg-blue-600 h-3 rounded-full transition-all duration-500"
                      style={{ width: `${stats.occupancyRate}%` }}
                    />
                  </div>
                </div>
                <div className="grid grid-cols-2 gap-4 pt-4 border-t">
                  <div>
                    <p className="text-sm text-gray-600">Total Capacity</p>
                    <p className="text-2xl font-semibold text-gray-900">
                      {stats.totalRooms > 0 ? stats.totalRooms : '-'}
                    </p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">Available</p>
                    <p className="text-2xl font-semibold text-green-600">
                      {stats.availableRooms}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </Card>

          {/* System Health */}
          <Card>
            <div className="p-6">
              <div className="flex items-center justify-between mb-4">
                <h3 className="text-lg font-semibold text-gray-900">System Health</h3>
                <FaCheckCircle className={`text-2xl ${stats.unresolvedAlerts === 0 ? 'text-green-500' : 'text-yellow-500'}`} />
              </div>
              <div className="space-y-3">
                <div className="flex items-center justify-between p-3 bg-blue-50 rounded-lg">
                  <div className="flex items-center gap-2">
                    <FaUsers className="text-blue-600" />
                    <span className="text-sm font-medium text-gray-700">Resident Management</span>
                  </div>
                  <span className="text-sm font-semibold text-blue-600">
                    {stats.activeResidents > 0 ? 'Active' : 'Empty'}
                  </span>
                </div>
                
                <div className="flex items-center justify-between p-3 bg-cyan-50 rounded-lg">
                  <div className="flex items-center gap-2">
                    <FaThermometerHalf className="text-cyan-600" />
                    <span className="text-sm font-medium text-gray-700">Sensor Monitoring</span>
                  </div>
                  <span className="text-sm font-semibold text-cyan-600">
                    {stats.recentSensors > 0 ? 'Active' : 'Idle'}
                  </span>
                </div>
                
                <div className="flex items-center justify-between p-3 bg-green-50 rounded-lg">
                  <div className="flex items-center gap-2">
                    <FaDoorOpen className="text-green-600" />
                    <span className="text-sm font-medium text-gray-700">Room Availability</span>
                  </div>
                  <span className="text-sm font-semibold text-green-600">
                    {stats.availableRooms > 0 ? `${stats.availableRooms} Available` : 'Full'}
                  </span>
                </div>
                
                <div className={`flex items-center justify-between p-3 rounded-lg ${
                  stats.unresolvedAlerts === 0 ? 'bg-green-50' : 'bg-red-50'
                }`}>
                  <div className="flex items-center gap-2">
                    <FaBell className={stats.unresolvedAlerts === 0 ? 'text-green-600' : 'text-red-600'} />
                    <span className="text-sm font-medium text-gray-700">Alert Status</span>
                  </div>
                  <span className={`text-sm font-semibold ${
                    stats.unresolvedAlerts === 0 ? 'text-green-600' : 'text-red-600'
                  }`}>
                    {stats.unresolvedAlerts === 0 ? 'All Clear' : `${stats.unresolvedAlerts} Active`}
                  </span>
                </div>
              </div>
            </div>
          </Card>
        </div>

        {/* Additional Alerts (non-critical) */}
        {stats.unresolvedAlerts > 0 && stats.criticalAlerts === 0 && (
          <div className="bg-yellow-50 border-l-4 border-yellow-500 p-4 rounded-r-lg">
            <div className="flex items-center gap-3">
              <FaBell className="text-yellow-600 text-2xl" />
              <div>
                <h3 className="font-semibold text-yellow-900">
                  {stats.unresolvedAlerts} Unresolved Alert{stats.unresolvedAlerts !== 1 ? 's' : ''}
                </h3>
                <p className="text-sm text-yellow-800">
                  Please review pending alerts when convenient.
                </p>
              </div>
            </div>
          </div>
        )}
      </div>
    </MainLayout>
  );
};

export default Dashboard;


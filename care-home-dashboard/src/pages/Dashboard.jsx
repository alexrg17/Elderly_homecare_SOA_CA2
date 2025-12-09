import { useAuth } from '../contexts/AuthContext.jsx';
import { FaUsers, FaDoorOpen, FaThermometerHalf, FaBell } from 'react-icons/fa';
import MainLayout from '../components/layout/MainLayout.jsx';
import PageHeader from '../components/common/PageHeader.jsx';
import StatCard from '../components/common/StatCard.jsx';
import Card from '../components/common/Card.jsx';

const Dashboard = () => {
  const { user } = useAuth();

  return (
    <MainLayout>
      <PageHeader
        title="Dashboard"
        subtitle={`Welcome back, ${user?.fullName || user?.username}!`}
      />
      
      <div className="p-6">
        {/* Stats Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
          <StatCard
            title="Total Residents"
            value="24"
            icon={FaUsers}
            color="blue"
            trend={{ value: '+2 this month', positive: true }}
          />
          <StatCard
            title="Available Rooms"
            value="6"
            icon={FaDoorOpen}
            color="green"
            trend={{ value: '75% occupied', positive: true }}
          />
          <StatCard
            title="Active Sensors"
            value="32"
            icon={FaThermometerHalf}
            color="cyan"
            trend={{ value: 'All operational', positive: true }}
          />
          <StatCard
            title="Active Alerts"
            value="3"
            icon={FaBell}
            color="red"
            trend={{ value: '2 high priority', positive: false }}
          />
        </div>

        {/* Welcome Card */}
        <Card title="System Status" subtitle="All systems operational">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
              <h3 className="font-semibold text-blue-900 mb-2">Phase 3 Complete</h3>
              <p className="text-sm text-blue-700">✅ Navigation & Layout working</p>
            </div>
            <div className="bg-green-50 border border-green-200 rounded-lg p-4">
              <h3 className="font-semibold text-green-900 mb-2">UI Components</h3>
              <p className="text-sm text-green-700">✅ Reusable components ready</p>
            </div>
            <div className="bg-purple-50 border border-purple-200 rounded-lg p-4">
              <h3 className="font-semibold text-purple-900 mb-2">Sidebar Navigation</h3>
              <p className="text-sm text-purple-700">✅ Multi-page routing enabled</p>
            </div>
            <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
              <h3 className="font-semibold text-yellow-900 mb-2">Dashboard Ready</h3>
              <p className="text-sm text-yellow-700">✅ Ready for more features</p>
            </div>
          </div>
        </Card>
      </div>
    </MainLayout>
  );
};

export default Dashboard;


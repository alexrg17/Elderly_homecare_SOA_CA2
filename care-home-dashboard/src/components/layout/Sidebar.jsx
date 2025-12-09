import { NavLink } from 'react-router-dom';
import { FaHome, FaUsers, FaDoorOpen, FaThermometerHalf, FaBell, FaSignOutAlt, FaHospital } from 'react-icons/fa';
import { useAuth } from '../../contexts/AuthContext.jsx';
import { useNavigate } from 'react-router-dom';

const Sidebar = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const navItems = [
    { path: '/', icon: FaHome, label: 'Dashboard' },
    { path: '/residents', icon: FaUsers, label: 'Residents' },
    { path: '/rooms', icon: FaDoorOpen, label: 'Rooms' },
    { path: '/sensors', icon: FaThermometerHalf, label: 'Sensors' },
    { path: '/alerts', icon: FaBell, label: 'Alerts' },
  ];

  return (
    <div className="h-screen w-64 bg-gray-900 text-white flex flex-col">
      {/* Logo/Brand */}
      <div className="p-6 border-b border-gray-700">
        <div className="flex items-center gap-3">
          <FaHospital className="text-blue-500 text-2xl" />
          <div>
            <h1 className="font-bold text-lg">Care Home</h1>
            <p className="text-xs text-gray-400">Monitoring System</p>
          </div>
        </div>
      </div>

      {/* Navigation */}
      <nav className="flex-1 p-4 overflow-y-auto">
        <ul className="space-y-2">
          {navItems.map((item) => (
            <li key={item.path}>
              <NavLink
                to={item.path}
                end={item.path === '/'}
                className={({ isActive }) =>
                  `flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
                    isActive
                      ? 'bg-blue-600 text-white'
                      : 'text-gray-300 hover:bg-gray-800 hover:text-white'
                  }`
                }
              >
                <item.icon className="text-lg" />
                <span className="font-medium">{item.label}</span>
              </NavLink>
            </li>
          ))}
        </ul>
      </nav>

      {/* User Info & Logout */}
      <div className="p-4 border-t border-gray-700">
        <div className="mb-3 px-2">
          <p className="text-sm font-medium truncate">{user?.fullName || user?.username}</p>
          <p className="text-xs text-gray-400">{user?.role}</p>
        </div>
        <button
          onClick={handleLogout}
          className="w-full flex items-center justify-center gap-2 px-4 py-2 bg-red-600 hover:bg-red-700 rounded-lg transition"
        >
          <FaSignOutAlt />
          <span>Logout</span>
        </button>
      </div>
    </div>
  );
};

export default Sidebar;


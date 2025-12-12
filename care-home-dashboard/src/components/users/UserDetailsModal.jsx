import { FaTimes, FaUser, FaEnvelope, FaUserShield, FaCalendar, FaCheckCircle, FaTimesCircle, FaKey } from 'react-icons/fa';
import Button from '../common/Button';

const UserDetailsModal = ({ user, onClose, onEdit, onResetPassword }) => {
  if (!user) return null;

  const getRoleBadgeColor = (role) => {
    switch (role) {
      case 'Admin':
        return 'bg-purple-100 text-purple-800 border-purple-300';
      case 'Caretaker':
        return 'bg-blue-100 text-blue-800 border-blue-300';
      case 'Nurse':
        return 'bg-green-100 text-green-800 border-green-300';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-300';
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200 bg-gradient-to-r from-blue-50 to-indigo-50">
          <div className="flex items-center gap-4">
            <div className="w-16 h-16 bg-gradient-to-br from-blue-400 to-blue-600 rounded-full flex items-center justify-center text-white font-bold text-2xl">
              {user.fullName.charAt(0).toUpperCase()}
            </div>
            <div>
              <h2 className="text-2xl font-bold text-gray-900">{user.fullName}</h2>
              <p className="text-gray-600">@{user.username}</p>
            </div>
          </div>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 transition"
          >
            <FaTimes size={24} />
          </button>
        </div>

        {/* Content */}
        <div className="p-6 space-y-6">
          {/* Status Badge */}
          <div className="flex items-center justify-center">
            {user.isActive ? (
              <span className="px-4 py-2 bg-green-100 text-green-800 rounded-full flex items-center gap-2 font-semibold">
                <FaCheckCircle /> Active Account
              </span>
            ) : (
              <span className="px-4 py-2 bg-red-100 text-red-800 rounded-full flex items-center gap-2 font-semibold">
                <FaTimesCircle /> Inactive Account
              </span>
            )}
          </div>

          {/* User Information Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Username */}
            <div className="bg-gray-50 p-4 rounded-lg">
              <div className="flex items-center gap-2 text-gray-600 mb-2">
                <FaUser className="text-blue-600" />
                <span className="text-sm font-medium">Username</span>
              </div>
              <p className="text-lg font-semibold text-gray-900">{user.username}</p>
            </div>

            {/* Email */}
            <div className="bg-gray-50 p-4 rounded-lg">
              <div className="flex items-center gap-2 text-gray-600 mb-2">
                <FaEnvelope className="text-blue-600" />
                <span className="text-sm font-medium">Email</span>
              </div>
              <p className="text-lg font-semibold text-gray-900 break-all">{user.email}</p>
            </div>

            {/* Role */}
            <div className="bg-gray-50 p-4 rounded-lg">
              <div className="flex items-center gap-2 text-gray-600 mb-2">
                <FaUserShield className="text-blue-600" />
                <span className="text-sm font-medium">Role</span>
              </div>
              <span className={`px-3 py-1 inline-flex text-sm font-semibold rounded-full border ${getRoleBadgeColor(user.role)}`}>
                {user.role}
              </span>
            </div>

            {/* Created Date */}
            <div className="bg-gray-50 p-4 rounded-lg">
              <div className="flex items-center gap-2 text-gray-600 mb-2">
                <FaCalendar className="text-blue-600" />
                <span className="text-sm font-medium">Member Since</span>
              </div>
              <p className="text-lg font-semibold text-gray-900">
                {new Date(user.createdAt).toLocaleDateString('en-US', {
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric'
                })}
              </p>
            </div>
          </div>

          {/* Role Permissions */}
          <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
            <h3 className="font-semibold text-blue-900 mb-3">Role Permissions</h3>
            <div className="space-y-2 text-sm text-blue-800">
              {user.role === 'Admin' && (
                <>
                  <p>✓ Full system access</p>
                  <p>✓ Manage all users via Admin Panel</p>
                  <p>✓ Full CRUD on residents, rooms, and sensors</p>
                  <p>✓ View, resolve, and delete alerts</p>
                  <p>✓ Access all reports and analytics</p>
                </>
              )}
              {user.role === 'Nurse' && (
                <>
                  <p>✓ Full CRUD on residents and rooms</p>
                  <p>✓ Manage sensor data</p>
                  <p>✓ View, resolve, and delete alerts</p>
                  <p>✓ Access reports and analytics</p>
                  <p>✗ No access to Admin Panel (User Management)</p>
                </>
              )}
              {user.role === 'Caretaker' && (
                <>
                  <p>✓ View residents and rooms (read-only)</p>
                  <p>✓ View sensor data (read-only)</p>
                  <p>✓ View alerts</p>
                  <p>✓ Resolve and delete alerts only</p>
                  <p>✗ Cannot add/edit/delete residents or rooms</p>
                  <p>✗ No access to Admin Panel</p>
                </>
              )}
            </div>
          </div>

          {/* Additional Info */}
          <div className="grid grid-cols-2 gap-4 text-center">
            <div className="bg-gradient-to-br from-purple-50 to-purple-100 p-4 rounded-lg">
              <p className="text-2xl font-bold text-purple-900">
                {Math.floor((new Date() - new Date(user.createdAt)) / (1000 * 60 * 60 * 24))}
              </p>
              <p className="text-xs text-purple-700 mt-1">Days Active</p>
            </div>
            <div className="bg-gradient-to-br from-indigo-50 to-indigo-100 p-4 rounded-lg">
              <p className="text-2xl font-bold text-indigo-900">
                {user.isActive ? '✓' : '✗'}
              </p>
              <p className="text-xs text-indigo-700 mt-1">Account Status</p>
            </div>
          </div>
        </div>

        {/* Footer */}
        <div className="flex gap-3 p-6 border-t border-gray-200 bg-gray-50">
          <Button onClick={onEdit} className="flex-1">
            Edit User
          </Button>
          {onResetPassword && (
            <Button onClick={onResetPassword} variant="primary" className="flex-1 bg-blue-600 hover:bg-blue-700">
              <FaKey className="inline mr-2" />
              Reset Password
            </Button>
          )}
          <Button variant="secondary" onClick={onClose}>
            Close
          </Button>
        </div>
      </div>
    </div>
  );
};

export default UserDetailsModal;


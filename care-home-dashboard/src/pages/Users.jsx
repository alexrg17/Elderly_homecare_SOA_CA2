import { useState, useEffect } from 'react';
import { FaUserPlus, FaEdit, FaTrash, FaUserShield, FaUserCheck, FaUserTimes, FaSearch, FaKey } from 'react-icons/fa';
import api from '../services/api';
import MainLayout from '../components/layout/MainLayout';
import PageHeader from '../components/common/PageHeader';
import Button from '../components/common/Button';
import Card from '../components/common/Card';
import LoadingSpinner from '../components/common/LoadingSpinner';
import UserFormModal from '../components/users/UserFormModal';
import UserDetailsModal from '../components/users/UserDetailsModal';
import ResetPasswordModal from '../components/users/ResetPasswordModal';
import ConfirmDialog from '../components/common/ConfirmDialog';

const Users = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showFormModal, setShowFormModal] = useState(false);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [showResetPasswordModal, setShowResetPasswordModal] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [roleFilter, setRoleFilter] = useState('All');

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const data = await api.getUsers();
      setUsers(data);
    } catch (error) {
      console.error('Error fetching users:', error);
      alert('Failed to fetch users');
    } finally {
      setLoading(false);
    }
  };

  const handleAddUser = () => {
    setSelectedUser(null);
    setShowFormModal(true);
  };

  const handleEditUser = (user) => {
    setSelectedUser(user);
    setShowFormModal(true);
  };

  const handleViewUser = (user) => {
    setSelectedUser(user);
    setShowDetailsModal(true);
  };

  const handleDeleteClick = (user) => {
    setSelectedUser(user);
    setShowDeleteDialog(true);
  };

  const handleDeleteConfirm = async () => {
    try {
      await api.deleteUser(selectedUser.id);
      fetchUsers();
      setShowDeleteDialog(false);
      alert('User deleted successfully');
    } catch (error) {
      alert(error.response?.data?.message || 'Failed to delete user');
    }
  };

  const handleFormSubmit = async (formData) => {
    try {
      if (selectedUser) {
        await api.updateUser(selectedUser.id, formData);
        alert('User updated successfully');
      } else {
        await api.register(formData);
        alert('User created successfully');
      }
      fetchUsers();
      setShowFormModal(false);
    } catch (error) {
      alert(error.response?.data?.message || `Failed to ${selectedUser ? 'update' : 'create'} user`);
      throw error;
    }
  };

  const toggleUserStatus = async (user) => {
    try {
      await api.updateUser(user.id, { isActive: !user.isActive });
      fetchUsers();
      alert(`User ${!user.isActive ? 'activated' : 'deactivated'} successfully`);
    } catch (error) {
      alert('Failed to update user status');
    }
  };

  const handleResetPasswordClick = (user) => {
    setSelectedUser(user);
    setShowResetPasswordModal(true);
  };

  const handleResetPasswordSubmit = async (newPassword) => {
    try {
      await api.resetPassword(selectedUser.id, newPassword);
      alert('Password reset successfully');
      setShowResetPasswordModal(false);
    } catch (error) {
      alert(error.response?.data?.message || 'Failed to reset password');
      throw error;
    }
  };

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

  const getRoleIcon = (role) => {
    switch (role) {
      case 'Admin':
        return <FaUserShield className="text-purple-600" />;
      case 'Caretaker':
        return <FaUserCheck className="text-blue-600" />;
      case 'Nurse':
        return <FaUserCheck className="text-green-600" />;
      default:
        return <FaUserCheck className="text-gray-600" />;
    }
  };

  const filteredUsers = users.filter(user => {
    const matchesSearch = 
      user.username.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.fullName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.email.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesRole = roleFilter === 'All' || user.role === roleFilter;
    
    return matchesSearch && matchesRole;
  });

  const getUserStats = () => {
    return {
      total: users.length,
      active: users.filter(u => u.isActive).length,
      inactive: users.filter(u => !u.isActive).length,
      admins: users.filter(u => u.role === 'Admin').length,
      caretakers: users.filter(u => u.role === 'Caretaker').length,
      nurses: users.filter(u => u.role === 'Nurse').length,
    };
  };

  const stats = getUserStats();

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
        title="User Management"
        subtitle="Manage system users and permissions"
        icon={FaUserShield}
        actions={
          <Button icon={FaUserPlus} onClick={handleAddUser}>
            Add User
          </Button>
        }
      />
      
      <div className="p-6 space-y-6">

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-6 gap-4">
        <Card className="bg-gradient-to-br from-blue-50 to-blue-100 border-blue-200">
          <div className="text-center">
            <p className="text-2xl font-bold text-blue-900">{stats.total}</p>
            <p className="text-sm text-blue-700">Total Users</p>
          </div>
        </Card>
        <Card className="bg-gradient-to-br from-green-50 to-green-100 border-green-200">
          <div className="text-center">
            <p className="text-2xl font-bold text-green-900">{stats.active}</p>
            <p className="text-sm text-green-700">Active</p>
          </div>
        </Card>
        <Card className="bg-gradient-to-br from-red-50 to-red-100 border-red-200">
          <div className="text-center">
            <p className="text-2xl font-bold text-red-900">{stats.inactive}</p>
            <p className="text-sm text-red-700">Inactive</p>
          </div>
        </Card>
        <Card className="bg-gradient-to-br from-purple-50 to-purple-100 border-purple-200">
          <div className="text-center">
            <p className="text-2xl font-bold text-purple-900">{stats.admins}</p>
            <p className="text-sm text-purple-700">Admins</p>
          </div>
        </Card>
        <Card className="bg-gradient-to-br from-indigo-50 to-indigo-100 border-indigo-200">
          <div className="text-center">
            <p className="text-2xl font-bold text-indigo-900">{stats.caretakers}</p>
            <p className="text-sm text-indigo-700">Caretakers</p>
          </div>
        </Card>
        <Card className="bg-gradient-to-br from-teal-50 to-teal-100 border-teal-200">
          <div className="text-center">
            <p className="text-2xl font-bold text-teal-900">{stats.nurses}</p>
            <p className="text-sm text-teal-700">Nurses</p>
          </div>
        </Card>
      </div>

      {/* Filters */}
      <Card>
        <div className="flex flex-col md:flex-row gap-4">
          <div className="flex-1 relative">
            <FaSearch className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
            <input
              type="text"
              placeholder="Search by username, name, or email..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <select
            value={roleFilter}
            onChange={(e) => setRoleFilter(e.target.value)}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option value="All">All Roles</option>
            <option value="Admin">Admin</option>
            <option value="Caretaker">Caretaker</option>
            <option value="Nurse">Nurse</option>
          </select>
        </div>
      </Card>

      {/* Users Table */}
      <Card>
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">User</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Role</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Created</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredUsers.map((user) => (
                <tr 
                  key={user.id} 
                  className="hover:bg-gray-50 cursor-pointer"
                  onClick={() => handleViewUser(user)}
                >
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="flex items-center gap-3">
                      <div className="w-10 h-10 bg-gradient-to-br from-blue-400 to-blue-600 rounded-full flex items-center justify-center text-white font-semibold">
                        {user.fullName.charAt(0).toUpperCase()}
                      </div>
                      <div>
                        <div className="font-medium text-gray-900">{user.fullName}</div>
                        <div className="text-sm text-gray-500">@{user.username}</div>
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="flex items-center gap-2">
                      {getRoleIcon(user.role)}
                      <span className={`px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full border ${getRoleBadgeColor(user.role)}`}>
                        {user.role}
                      </span>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                    {user.email}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        toggleUserStatus(user);
                      }}
                      className={`px-3 py-1 inline-flex items-center gap-1 text-xs leading-5 font-semibold rounded-full transition-colors ${
                        user.isActive
                          ? 'bg-green-100 text-green-800 hover:bg-green-200'
                          : 'bg-red-100 text-red-800 hover:bg-red-200'
                      }`}
                    >
                      {user.isActive ? (
                        <>
                          <FaUserCheck /> Active
                        </>
                      ) : (
                        <>
                          <FaUserTimes /> Inactive
                        </>
                      )}
                    </button>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                    {new Date(user.createdAt).toLocaleDateString()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <div className="flex justify-end gap-2" onClick={(e) => e.stopPropagation()}>
                      <Button
                        size="sm"
                        variant="secondary"
                        onClick={() => handleEditUser(user)}
                        className="flex items-center gap-1"
                        title="Edit user"
                      >
                        <FaEdit /> Edit
                      </Button>
                      <Button
                        size="sm"
                        variant="primary"
                        onClick={() => handleResetPasswordClick(user)}
                        className="flex items-center gap-1 bg-blue-600 hover:bg-blue-700"
                        title="Reset password"
                      >
                        <FaKey /> Reset
                      </Button>
                      <Button
                        size="sm"
                        variant="danger"
                        onClick={() => handleDeleteClick(user)}
                        className="flex items-center gap-1"
                        title="Delete user"
                      >
                        <FaTrash /> Delete
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {filteredUsers.length === 0 && (
            <div className="text-center py-12">
              <p className="text-gray-500">No users found</p>
            </div>
          )}
        </div>
      </Card>

      {/* Modals */}
      {showFormModal && (
        <UserFormModal
          user={selectedUser}
          onClose={() => setShowFormModal(false)}
          onSubmit={handleFormSubmit}
        />
      )}

      {showDetailsModal && (
        <UserDetailsModal
          user={selectedUser}
          onClose={() => setShowDetailsModal(false)}
          onEdit={() => {
            setShowDetailsModal(false);
            handleEditUser(selectedUser);
          }}
          onResetPassword={() => {
            setShowDetailsModal(false);
            handleResetPasswordClick(selectedUser);
          }}
        />
      )}

      {showResetPasswordModal && (
        <ResetPasswordModal
          user={selectedUser}
          onClose={() => setShowResetPasswordModal(false)}
          onSubmit={handleResetPasswordSubmit}
        />
      )}

      {showDeleteDialog && (
        <ConfirmDialog
          title="Delete User"
          message={`Are you sure you want to delete ${selectedUser?.fullName}? This action cannot be undone.`}
          confirmText="Delete"
          onConfirm={handleDeleteConfirm}
          onCancel={() => setShowDeleteDialog(false)}
        />
      )}
      </div>
    </MainLayout>
  );
};

export default Users;


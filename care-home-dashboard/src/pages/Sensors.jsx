import { useState, useEffect } from 'react';
import MainLayout from '../components/layout/MainLayout.jsx';
import PageHeader from '../components/common/PageHeader.jsx';
import Card from '../components/common/Card.jsx';
import Table from '../components/common/Table.jsx';
import Badge from '../components/common/Badge.jsx';
import LoadingSpinner from '../components/common/LoadingSpinner.jsx';
import StatCard from '../components/common/StatCard.jsx';
import { FaThermometerHalf, FaTint, FaExclamationTriangle, FaCheckCircle } from 'react-icons/fa';
import sensorsService from '../services/sensorsService';
import roomsService from '../services/roomsService';

const Sensors = () => {
  const [sensorData, setSensorData] = useState([]);
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedRoom, setSelectedRoom] = useState('all');
  const [stats, setStats] = useState({
    avgTemp: 0,
    avgHumidity: 0,
    abnormalReadings: 0,
    totalReadings: 0
  });

  useEffect(() => {
    fetchData();
  }, []);

  useEffect(() => {
    calculateStats();
  }, [sensorData]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [sensorsData, roomsData] = await Promise.all([
        sensorsService.getRecent(100),
        roomsService.getAll()
      ]);
      setSensorData(sensorsData);
      setRooms(roomsData);
      setError(null);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to fetch sensor data');
      console.error('Error fetching data:', err);
    } finally {
      setLoading(false);
    }
  };

  const calculateStats = () => {
    if (sensorData.length === 0) return;

    const filteredData = selectedRoom === 'all' 
      ? sensorData 
      : sensorData.filter(s => s.roomId === parseInt(selectedRoom));

    const avgTemp = filteredData.reduce((sum, s) => sum + s.temperature, 0) / filteredData.length;
    const avgHumidity = filteredData.reduce((sum, s) => sum + s.humidity, 0) / filteredData.length;
    
    // Abnormal readings: temp < 18 or > 26, humidity < 30 or > 60
    const abnormal = filteredData.filter(s => 
      s.temperature < 18 || s.temperature > 26 || s.humidity < 30 || s.humidity > 60
    ).length;

    setStats({
      avgTemp: avgTemp.toFixed(1),
      avgHumidity: avgHumidity.toFixed(1),
      abnormalReadings: abnormal,
      totalReadings: filteredData.length
    });
  };

  const isAbnormal = (temp, humidity) => {
    return temp < 18 || temp > 26 || humidity < 30 || humidity > 60;
  };

  const formatDateTime = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleString();
  };

  const filteredSensorData = selectedRoom === 'all' 
    ? sensorData 
    : sensorData.filter(s => s.roomId === parseInt(selectedRoom));

  const columns = [
    { 
      header: 'Timestamp', 
      accessor: (row) => formatDateTime(row.timestamp)
    },
    { header: 'Room', accessor: 'roomNumber' },
    { 
      header: 'Temperature (°C)', 
      accessor: (row) => (
        <div className="flex items-center gap-2">
          <FaThermometerHalf className={row.temperature < 18 || row.temperature > 26 ? 'text-red-500' : 'text-blue-500'} />
          <span className={row.temperature < 18 || row.temperature > 26 ? 'font-semibold text-red-600' : ''}>
            {row.temperature}°C
          </span>
        </div>
      )
    },
    { 
      header: 'Humidity (%)', 
      accessor: (row) => (
        <div className="flex items-center gap-2">
          <FaTint className={row.humidity < 30 || row.humidity > 60 ? 'text-red-500' : 'text-blue-500'} />
          <span className={row.humidity < 30 || row.humidity > 60 ? 'font-semibold text-red-600' : ''}>
            {row.humidity}%
          </span>
        </div>
      )
    },
    { header: 'Sensor Type', accessor: (row) => row.sensorType || 'Standard' },
    {
      header: 'Status',
      accessor: (row) => (
        isAbnormal(row.temperature, row.humidity) ? (
          <Badge variant="error">
            <FaExclamationTriangle className="inline mr-1" />
            Abnormal
          </Badge>
        ) : (
          <Badge variant="success">
            <FaCheckCircle className="inline mr-1" />
            Normal
          </Badge>
        )
      )
    }
  ];

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
        title="Sensor Data"
        subtitle="Monitor environmental sensor readings"
        icon={FaThermometerHalf}
      />
      
      <div className="p-6 space-y-6">
        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
            {error}
          </div>
        )}

        {/* Stats Overview */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
          <StatCard
            title="Avg Temperature"
            value={`${stats.avgTemp}°C`}
            icon={FaThermometerHalf}
            trend={parseFloat(stats.avgTemp) >= 20 && parseFloat(stats.avgTemp) <= 24 ? 'up' : 'down'}
            color="blue"
          />
          <StatCard
            title="Avg Humidity"
            value={`${stats.avgHumidity}%`}
            icon={FaTint}
            trend={parseFloat(stats.avgHumidity) >= 40 && parseFloat(stats.avgHumidity) <= 50 ? 'up' : 'down'}
            color="cyan"
          />
          <StatCard
            title="Abnormal Readings"
            value={stats.abnormalReadings}
            icon={FaExclamationTriangle}
            trend={stats.abnormalReadings === 0 ? 'up' : 'down'}
            color={stats.abnormalReadings > 0 ? 'red' : 'green'}
          />
          <StatCard
            title="Total Readings"
            value={stats.totalReadings}
            icon={FaCheckCircle}
            color="green"
          />
        </div>

        {/* Filter */}
        <Card>
          <div className="p-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Filter by Room
            </label>
            <select
              value={selectedRoom}
              onChange={(e) => setSelectedRoom(e.target.value)}
              className="w-full md:w-64 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="all">All Rooms</option>
              {rooms.map(room => (
                <option key={room.id} value={room.id}>
                  {room.roomNumber} - {room.roomName || 'No name'}
                </option>
              ))}
            </select>
          </div>
        </Card>

        {/* Sensor Data Table */}
        <Card title="Recent Sensor Readings">
          {filteredSensorData.length === 0 ? (
            <div className="text-center py-12">
              <FaThermometerHalf className="mx-auto h-12 w-12 text-gray-400 mb-4" />
              <h3 className="text-lg font-semibold text-gray-900 mb-2">No sensor data found</h3>
              <p className="text-gray-600">Sensor readings will appear here as they are collected.</p>
            </div>
          ) : (
            <Table columns={columns} data={filteredSensorData} />
          )}
        </Card>
      </div>
    </MainLayout>
  );
};

export default Sensors;


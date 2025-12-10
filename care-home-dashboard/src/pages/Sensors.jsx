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
  const [showAbnormalModal, setShowAbnormalModal] = useState(false);
  const [stats, setStats] = useState({
    avgTemp: 0,
    avgHumidity: 0,
    abnormalReadings: 0,
    roomsMonitored: 0
  });

  useEffect(() => {
    fetchData();
    
    // Auto-refresh every 10 seconds
    const intervalId = setInterval(() => {
      fetchData();
    }, 10000);
    
    // Cleanup interval on unmount
    return () => clearInterval(intervalId);
  }, []);

  useEffect(() => {
    calculateStats();
  }, [sensorData]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [sensorsData, roomsData] = await Promise.all([
        sensorsService.getRecent(50), // Only get last 50 readings for recent history
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

    // Count unique rooms with sensor data
    const uniqueRooms = new Set(sensorData.map(s => s.roomId)).size;

    setStats({
      avgTemp: avgTemp.toFixed(1),
      avgHumidity: avgHumidity.toFixed(1),
      abnormalReadings: abnormal,
      roomsMonitored: uniqueRooms
    });
  };

  const isAbnormal = (temp, humidity) => {
    return temp < 18 || temp > 26 || humidity < 30 || humidity > 60;
  };

  const formatDateTime = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleString();
  };

  const getLatestReadingPerRoom = () => {
    const latestByRoom = {};
    sensorData.forEach(reading => {
      if (!latestByRoom[reading.roomId] || 
          new Date(reading.timestamp) > new Date(latestByRoom[reading.roomId].timestamp)) {
        latestByRoom[reading.roomId] = reading;
      }
    });
    return Object.values(latestByRoom);
  };

  const getAbnormalReadings = () => {
    return sensorData.filter(reading => 
      isAbnormal(reading.temperature, reading.humidity)
    ).sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp)); // Most recent first
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
        subtitle="Real-time environmental monitoring • Auto-refreshes every 10 seconds"
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
          <div 
            onClick={() => stats.abnormalReadings > 0 && setShowAbnormalModal(true)}
            className={stats.abnormalReadings > 0 ? 'cursor-pointer hover:opacity-80 transition-opacity' : ''}
            title={stats.abnormalReadings > 0 ? 'Click to view abnormal readings' : ''}
          >
            <StatCard
              title="Abnormal Readings"
              value={stats.abnormalReadings}
              icon={FaExclamationTriangle}
              trend={stats.abnormalReadings === 0 ? 'up' : 'down'}
              color={stats.abnormalReadings > 0 ? 'red' : 'green'}
              subtitle={stats.abnormalReadings > 0 ? 'Click to view' : 'All clear'}
            />
          </div>
          <StatCard
            title="Rooms Monitored"
            value={stats.roomsMonitored}
            icon={FaCheckCircle}
            color="green"
            subtitle="Active sensors"
          />
        </div>

        {/* Current Status - Latest reading per room */}
        <Card title="Current Room Status">
          <div className="p-6">
            {sensorData.length === 0 ? (
              <p className="text-center text-gray-500">No sensor data available</p>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {getLatestReadingPerRoom().map(reading => (
                  <div 
                    key={reading.id}
                    className={`border rounded-lg p-4 ${
                      isAbnormal(reading.temperature, reading.humidity)
                        ? 'border-red-300 bg-red-50'
                        : 'border-green-300 bg-green-50'
                    }`}
                  >
                    <div className="flex items-center justify-between mb-2">
                      <h3 className="font-semibold text-lg">Room {reading.roomNumber}</h3>
                      {isAbnormal(reading.temperature, reading.humidity) ? (
                        <Badge variant="error">
                          <FaExclamationTriangle className="inline mr-1" />
                          Abnormal
                        </Badge>
                      ) : (
                        <Badge variant="success">
                          <FaCheckCircle className="inline mr-1" />
                          Normal
                        </Badge>
                      )}
                    </div>
                    
                    <div className="space-y-2">
                      <div className="flex items-center justify-between">
                        <div className="flex items-center gap-2">
                          <FaThermometerHalf className={
                            reading.temperature < 18 || reading.temperature > 26
                              ? 'text-red-600'
                              : 'text-blue-600'
                          } />
                          <span className="text-sm text-gray-600">Temperature</span>
                        </div>
                        <span className={`font-bold ${
                          reading.temperature < 18 || reading.temperature > 26
                            ? 'text-red-600'
                            : 'text-gray-900'
                        }`}>
                          {reading.temperature}°C
                        </span>
                      </div>
                      
                      <div className="flex items-center justify-between">
                        <div className="flex items-center gap-2">
                          <FaTint className={
                            reading.humidity < 30 || reading.humidity > 60
                              ? 'text-red-600'
                              : 'text-blue-600'
                          } />
                          <span className="text-sm text-gray-600">Humidity</span>
                        </div>
                        <span className={`font-bold ${
                          reading.humidity < 30 || reading.humidity > 60
                            ? 'text-red-600'
                            : 'text-gray-900'
                        }`}>
                          {reading.humidity}%
                        </span>
                      </div>
                      
                      <div className="pt-2 border-t border-gray-300 mt-2">
                        <p className="text-xs text-gray-500">
                          Last updated: {new Date(reading.timestamp).toLocaleTimeString()}
                        </p>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </Card>

        {/* Filter */}
        <Card>
          <div className="p-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Filter History by Room
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

        {/* Sensor Data Table - Recent History */}
        <Card title="Recent History (Last 50 Readings)">
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

      {/* Abnormal Readings Modal */}
      {showAbnormalModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl max-w-4xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6 border-b border-gray-200 bg-gradient-to-r from-red-500 to-red-600">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <FaExclamationTriangle className="text-white text-3xl" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">
                      Abnormal Readings
                    </h3>
                    <p className="text-red-100">
                      {getAbnormalReadings().length} abnormal reading(s) detected in last 50 logs
                    </p>
                  </div>
                </div>
                <button
                  onClick={() => setShowAbnormalModal(false)}
                  className="text-white hover:text-gray-200 text-2xl font-bold"
                >
                  ×
                </button>
              </div>
            </div>
            
            <div className="p-6">
              {getAbnormalReadings().length === 0 ? (
                <div className="text-center py-12">
                  <FaCheckCircle className="mx-auto h-12 w-12 text-green-500 mb-4" />
                  <h3 className="text-lg font-semibold text-gray-900 mb-2">No Abnormal Readings</h3>
                  <p className="text-gray-600">All sensor readings are within normal ranges.</p>
                </div>
              ) : (
                <div className="space-y-4">
                  <div className="bg-yellow-50 border-l-4 border-yellow-500 p-4 rounded-r-lg mb-4">
                    <div className="flex items-start">
                      <FaExclamationTriangle className="text-yellow-600 text-xl mt-1 mr-3" />
                      <div>
                        <h4 className="font-semibold text-yellow-900">Abnormal Range Definitions:</h4>
                        <ul className="text-sm text-yellow-800 mt-1 space-y-1">
                          <li>• Temperature: &lt; 18°C or &gt; 26°C</li>
                          <li>• Humidity: &lt; 30% or &gt; 60%</li>
                        </ul>
                      </div>
                    </div>
                  </div>

                  {getAbnormalReadings().map((reading, index) => (
                    <div 
                      key={reading.id}
                      className="border border-red-300 rounded-lg p-4 bg-red-50 hover:bg-red-100 transition-colors"
                    >
                      <div className="flex items-start justify-between mb-3">
                        <div>
                          <div className="flex items-center gap-2 mb-1">
                            <Badge variant="error">
                              <FaExclamationTriangle className="inline mr-1" />
                              Abnormal
                            </Badge>
                            <span className="font-semibold text-gray-900">
                              Room {reading.roomNumber}
                            </span>
                          </div>
                          <p className="text-sm text-gray-600">
                            {formatDateTime(reading.timestamp)}
                          </p>
                        </div>
                        <span className="text-sm text-gray-500">
                          #{index + 1}
                        </span>
                      </div>

                      <div className="grid grid-cols-2 gap-4">
                        <div className="flex items-center justify-between bg-white rounded p-3 border border-red-200">
                          <div className="flex items-center gap-2">
                            <FaThermometerHalf className={
                              reading.temperature < 18 || reading.temperature > 26
                                ? 'text-red-600 text-xl'
                                : 'text-blue-600 text-xl'
                            } />
                            <div>
                              <p className="text-xs text-gray-500">Temperature</p>
                              <p className={`text-lg font-bold ${
                                reading.temperature < 18 || reading.temperature > 26
                                  ? 'text-red-600'
                                  : 'text-gray-900'
                              }`}>
                                {reading.temperature}°C
                              </p>
                            </div>
                          </div>
                          {(reading.temperature < 18 || reading.temperature > 26) && (
                            <span className="text-xs text-red-600 font-semibold">
                              {reading.temperature < 18 ? 'Too Cold' : 'Too Hot'}
                            </span>
                          )}
                        </div>

                        <div className="flex items-center justify-between bg-white rounded p-3 border border-red-200">
                          <div className="flex items-center gap-2">
                            <FaTint className={
                              reading.humidity < 30 || reading.humidity > 60
                                ? 'text-red-600 text-xl'
                                : 'text-blue-600 text-xl'
                            } />
                            <div>
                              <p className="text-xs text-gray-500">Humidity</p>
                              <p className={`text-lg font-bold ${
                                reading.humidity < 30 || reading.humidity > 60
                                  ? 'text-red-600'
                                  : 'text-gray-900'
                              }`}>
                                {reading.humidity}%
                              </p>
                            </div>
                          </div>
                          {(reading.humidity < 30 || reading.humidity > 60) && (
                            <span className="text-xs text-red-600 font-semibold">
                              {reading.humidity < 30 ? 'Too Dry' : 'Too Humid'}
                            </span>
                          )}
                        </div>
                      </div>

                      {reading.notes && (
                        <div className="mt-3 pt-3 border-t border-red-200">
                          <p className="text-sm text-gray-600">
                            <span className="font-semibold">Note:</span> {reading.notes}
                          </p>
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              )}

              <div className="mt-6 pt-4 border-t">
                <button
                  onClick={() => setShowAbnormalModal(false)}
                  className="w-full px-4 py-2 bg-gray-600 text-white rounded-lg hover:bg-gray-700 transition-colors font-medium"
                >
                  Close
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </MainLayout>
  );
};

export default Sensors;


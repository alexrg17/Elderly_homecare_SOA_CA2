import MainLayout from '../components/layout/MainLayout.jsx';
import PageHeader from '../components/common/PageHeader.jsx';
import Card from '../components/common/Card.jsx';

const Sensors = () => {
  return (
    <MainLayout>
      <PageHeader
        title="Sensor Data"
        subtitle="Monitor environmental sensor readings"
      />
      
      <div className="p-6">
        <Card>
          <div className="text-center py-12">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Sensor Monitoring</h3>
            <p className="text-gray-600 mb-4">
              This page will display temperature, humidity, and other sensor data with charts.
            </p>
            <p className="text-sm text-gray-500">
              Coming in future phases...
            </p>
          </div>
        </Card>
      </div>
    </MainLayout>
  );
};

export default Sensors;


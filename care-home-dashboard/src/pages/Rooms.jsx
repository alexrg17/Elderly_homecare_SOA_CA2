import MainLayout from '../components/layout/MainLayout.jsx';
import PageHeader from '../components/common/PageHeader.jsx';
import Card from '../components/common/Card.jsx';
import Button from '../components/common/Button.jsx';
import { FaPlus } from 'react-icons/fa';

const Rooms = () => {
  return (
    <MainLayout>
      <PageHeader
        title="Rooms"
        subtitle="Manage care home rooms and occupancy"
        actions={
          <Button icon={FaPlus}>
            Add Room
          </Button>
        }
      />
      
      <div className="p-6">
        <Card>
          <div className="text-center py-12">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Room Management</h3>
            <p className="text-gray-600 mb-4">
              This page will display room status, occupancy, and assignments.
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

export default Rooms;


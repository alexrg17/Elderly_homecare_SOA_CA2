import MainLayout from '../components/layout/MainLayout.jsx';
import PageHeader from '../components/common/PageHeader.jsx';
import Card from '../components/common/Card.jsx';
import Button from '../components/common/Button.jsx';
import { FaPlus } from 'react-icons/fa';

const Residents = () => {
  return (
    <MainLayout>
      <PageHeader
        title="Residents"
        subtitle="Manage elderly care home residents"
        actions={
          <Button icon={FaPlus}>
            Add Resident
          </Button>
        }
      />
      
      <div className="p-6">
        <Card>
          <div className="text-center py-12">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Residents Management</h3>
            <p className="text-gray-600 mb-4">
              This page will display and manage all residents in the care home.
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

export default Residents;


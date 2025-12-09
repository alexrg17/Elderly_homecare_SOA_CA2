import MainLayout from '../components/layout/MainLayout.jsx';
import PageHeader from '../components/common/PageHeader.jsx';
import Card from '../components/common/Card.jsx';

const Alerts = () => {
  return (
    <MainLayout>
      <PageHeader
        title="Alerts"
        subtitle="View and manage system alerts"
      />
      
      <div className="p-6">
        <Card>
          <div className="text-center py-12">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Alert Management</h3>
            <p className="text-gray-600 mb-4">
              This page will display active alerts and allow you to manage them.
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

export default Alerts;


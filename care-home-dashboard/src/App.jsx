import { useState, useEffect } from 'react'

function App() {
  const [apiStatus, setApiStatus] = useState('Checking...')

  useEffect(() => {
    // Simple API connectivity test
    const checkAPI = async () => {
      try {
        const response = await fetch('http://localhost:5000/api/rooms')
        if (response.ok) {
          setApiStatus('✅ API Connected Successfully!')
        } else {
          setApiStatus('⚠️ API returned error: ' + response.status)
        }
      } catch (error) {
        setApiStatus('❌ API not reachable. Make sure the backend is running on port 5000.')
      }
    }
    
    checkAPI()
  }, [])

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center">
      <div className="max-w-2xl w-full p-8">
        <div className="bg-white rounded-lg shadow-lg p-8">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            🏥 Care Home Dashboard
          </h1>
          <p className="text-gray-600 mb-6">
            Elderly Care Home Monitoring System
          </p>
          
          <div className="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6">
            <p className="text-sm font-semibold text-blue-900 mb-2">API Status:</p>
            <p className="text-blue-700">{apiStatus}</p>
          </div>

          <div className="space-y-3 text-sm text-gray-600">
            <div className="flex items-center gap-2">
              <span className="text-green-500">✓</span>
              <span>React + Vite configured</span>
            </div>
            <div className="flex items-center gap-2">
              <span className="text-green-500">✓</span>
              <span>TailwindCSS installed</span>
            </div>
            <div className="flex items-center gap-2">
              <span className="text-green-500">✓</span>
              <span>Dependencies installed</span>
            </div>
            <div className="flex items-center gap-2">
              <span className="text-green-500">✓</span>
              <span>Folder structure created</span>
            </div>
          </div>

          <div className="mt-6 pt-6 border-t border-gray-200">
            <p className="text-xs text-gray-500">
              Phase 1: Project Setup Complete! ✨
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}

export default App


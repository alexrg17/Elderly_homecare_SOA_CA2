# Elderly Care Home Management System

A complete web application for managing elderly care homes, including a REST API backend and a React dashboard. The system helps staff monitor residents, manage rooms, track environmental sensors, and respond to alerts.

## 📋 What Is This Project?

This is a full-stack care home management system built for my Service Oriented Architecture course assignment. It consists of:

1. **Backend API** - Built with ASP.NET Core, handles all data and business logic
2. **Web Dashboard** - Built with React, provides a user-friendly interface for staff
3. **SQLite Database** - Stores all residents, rooms, sensor data, and alerts
4. **Sensor Simulation** - Python script that simulates IoT temperature/humidity sensors

The system allows care home staff to manage residents, assign them to rooms, monitor environmental conditions, and respond to alerts when something goes wrong (like abnormal temperature or humidity).

---

## 🚀 How to Run the Project

### Prerequisites

You need to have these installed on your computer:
- .NET 8.0 SDK (for the backend API)
- Node.js and npm (for the frontend dashboard)
- Python 3.x (optional, for sensor simulation)

### Step 1: Start the Backend API

1. Open a terminal
2. Navigate to the CA2_SOA folder:
   ```bash
   cd /Users/alex/RiderProjects/CA2_SOA/CA2_SOA
   ```
3. Run the API:
   ```bash
   dotnet run
   ```
4. You should see something like:
   ```
   Now listening on: http://localhost:5000
   ```
5. Keep this terminal open - the API needs to stay running

### Step 2: Start the Frontend Dashboard

1. Open a **new** terminal (keep the first one running)
2. Navigate to the dashboard folder:
   ```bash
   cd /Users/alex/RiderProjects/CA2_SOA/care-home-dashboard
   ```
3. Install dependencies (only needed the first time):
   ```bash
   npm install
   ```
4. Run the dashboard:
   ```bash
   npm run dev
   ```
5. You should see:
   ```
   Local: http://localhost:5173/
   ```
6. Open your browser and go to `http://localhost:5173`

### Step 3: Simulate Sensor Data (Optional)

If you want to see live sensor readings and automatic alerts:

1. Open a **third** terminal
2. Navigate to the project root:
   ```bash
   cd /Users/alex/RiderProjects/CA2_SOA
   ```
3. Run the sensor simulator:
   ```bash
   python3 simulate_sensors.py
   ```
4. This will send temperature/humidity readings every 10 seconds

---

## 👥 User Accounts & Passwords

The system has three types of users with different permissions. Here are the test accounts:

### 🔴 Admin
**Username:** `admin`  
**Password:** `Admin123!`

**What admins can do:**
- Everything! Full access to all features
- Manage user accounts (create, edit, delete users)
- Add, edit, and delete residents
- Add, edit, and delete rooms
- View and resolve alerts
- Access the Admin Panel to manage all users
- Reset passwords for other users

### 🟢 Nurse
**Username:** `nurse`  
**Password:** `Nurse123!`

**What nurses can do:**
- Add, edit, and delete residents
- Add, edit, and delete rooms
- Assign residents to rooms
- View and resolve alerts
- View sensor data
- **Cannot** access the Admin Panel
- **Cannot** manage user accounts

### 🟡 Caretaker
**Username:** `caretaker1`  
**Password:** `Caretaker123!`

**What caretakers can do:**
- View all residents (read-only)
- View all rooms (read-only)
- View and resolve alerts
- View sensor data
- **Cannot** add, edit, or delete residents or rooms
- **Cannot** access the Admin Panel
- **Cannot** manage user accounts

---

## 🧪 Testing with Postman

The project includes ready-made Postman tests to verify all API endpoints work correctly.

### How to Run Postman Tests

1. **Open Postman** on your computer

2. **Import the Collection**
   - Click "Import" in Postman
   - Select the file: `CareHomeAPI.postman_collection.json`
   - This contains all the test requests

3. **Import the Environment**
   - Click the "Environments" tab
   - Click "Import"
   - Select the file: `CareHomeAPI.postman_environment.json`
   - This sets up the correct server URL (http://localhost:5000/api)

4. **Select the Environment**
   - In the top-right corner, select "Elderly Care Home - Local" from the dropdown

5. **Run the Tests**
   - Make sure your backend API is running (`dotnet run` in CA2_SOA folder)
   - Open the "Care Home API" collection
   - Click on "Authentication" folder
   - Run the "Login - Admin" request first
   - The token will automatically be saved
   - Now you can run any other request in the collection

### What Tests Are Included?

The Postman collection tests all these features:
- **Authentication**: Login and registration
- **Users**: Get all users, get user by ID, update user
- **Rooms**: Create, read, update, delete rooms
- **Residents**: Create, read, update, delete residents
- **Sensor Data**: Get readings, get recent data, get by room
- **Alerts**: Get alerts, resolve alerts, filter by severity

All tests should pass if the API is running correctly.

---

## 📊 What's Inside the Dashboard?

When you log in to the web dashboard, here's what you can do:

### Dashboard (Home Page)
- See quick statistics: total residents, rooms, recent sensor readings
- View active alerts that need attention
- See abnormal sensor readings

### Residents Page
- View all residents in a table
- Add new residents
- Edit resident details (name, age, medical conditions, emergency contact)
- Delete residents (admin/nurse only)
- Assign residents to rooms
- Click on a resident to see full details in a popup

### Rooms Page
- View all rooms with their status (Available, Occupied, Full)
- Add new rooms
- Edit room details (room number, floor, capacity)
- Delete rooms (admin/nurse only)
- See how many residents are in each room
- Click on a room to see full details including notes

### Sensor Data Page
- View the last 50 sensor readings from all rooms
- See temperature and humidity levels
- Readings update automatically if you run the Python simulator
- Abnormal readings are highlighted in red
- Click "Abnormal Readings" to filter and see only problematic readings

### Alerts Page
- View all alerts (high temperature, low humidity, etc.)
- Filter alerts by status (Active or Resolved)
- Mark alerts as resolved and add notes on how you fixed them
- Click on an alert to see full details
- Alerts are automatically created when sensors detect problems

### Admin Panel (Admins Only)
- Manage all user accounts
- Create new users (admin, nurse, or caretaker)
- Edit user roles and details
- Deactivate/reactivate user accounts
- Reset passwords for users who forgot them
- Delete user accounts

---

## 🏗️ Project Structure

```
CA2_SOA/
├── CA2_SOA/                          # Backend API
│   ├── Controllers/                  # API endpoints
│   ├── Models/                       # Database tables
│   ├── DTOs/                         # Data transfer objects
│   ├── Repositories/                 # Database access layer
│   ├── Services/                     # Business logic
│   ├── CareHomeDB.db                 # SQLite database file (local dev only, gitignored)
│   └── Program.cs                    # API startup
│
├── care-home-dashboard/              # Frontend React app
│   ├── src/
│   │   ├── components/               # Reusable UI components
│   │   ├── pages/                    # Dashboard pages
│   │   ├── services/                 # API calls
│   │   └── contexts/                 # Authentication state
│   └── package.json                  # Dependencies
│
├── CA2_SOA.Tests/                    # Unit tests
│   ├── Controllers/                  # Controller tests
│   ├── Repositories/                 # Repository tests
│   └── Services/                     # Service tests
│
├── simulate_sensors.py               # IoT sensor simulator
├── CareHomeAPI.postman_collection.json    # Postman tests
└── CareHomeAPI.postman_environment.json   # Postman environment
```

---

## 🔧 Technologies Used

### Backend
- **ASP.NET Core 8.0** - REST API framework
- **Entity Framework Core** - Database access (ORM)
- **SQLite** - Lightweight database
- **JWT Authentication** - Secure login tokens
- **BCrypt** - Password hashing for security

### Frontend
- **React 18** - UI framework
- **React Router** - Page navigation
- **Axios** - API calls
- **Tailwind CSS** - Styling
- **Vite** - Fast development server

### Testing
- **xUnit** - Unit testing framework
- **Moq** - Mocking for tests
- **Postman** - API integration testing

### Other
- **Python 3** - Sensor simulation script

---

## 📚 Key Features

### 1. Complete CRUD Operations
Every resource (users, rooms, residents, sensors, alerts) can be Created, Read, Updated, and Deleted through the API.

### 2. Role-Based Access Control
Three user roles with different permission levels ensure staff only access what they need.

### 3. Real-Time Sensor Monitoring
Environmental sensors track temperature and humidity in each room. Abnormal readings trigger automatic alerts.

### 4. Alert System
When something goes wrong (abnormal temperature, humidity, or manual alerts), staff are notified and can document how they resolved the issue.

### 5. Room Management
Track room capacity, occupancy status, and which residents are assigned to each room.

### 6. Responsive Design
The dashboard works on desktop and mobile devices.

---

## 🧪 Running Unit Tests

The project includes automated unit tests for the backend:

```bash
cd /Users/alex/RiderProjects/CA2_SOA/CA2_SOA.Tests
dotnet test
```

This will run all tests and show you which ones passed or failed.

---

## 📝 CA Requirements Met

This project fulfills all the assignment requirements:

✅ **1. Minimum 4 CRUD Services (30%)**
- 5 complete CRUD services: Users, Rooms, Residents, SensorData, Alerts

✅ **2. Authentication & Authorization (10%)**
- JWT token-based authentication
- Role-based access control (Admin, Nurse, Caretaker)

✅ **3. Persistent Storage & Design Patterns (20%)**
- SQLite database with 5 tables
- One-to-many relationships (Room → Residents, Room → Sensors, Room → Alerts)
- Repository Pattern for data access
- DTOs separate from database models
- Dependency Injection throughout

✅ **4. Testing (10%)**
- Postman collection with comprehensive API tests
- Unit tests for controllers, repositories, and services

✅ **5. Extra Features (20%)**
- Full React web dashboard
- Real-time sensor simulation
- Advanced filtering and search
- Alert resolution workflow
- User management panel
- Role-based UI (different views for different roles)

✅ **6. Deployment Ready**
- Can be deployed to Azure, AWS, or DKIT cloud
- Database migrations included
- Environment configuration set up

---

## 🐛 Troubleshooting

### API won't start
- Make sure .NET 8.0 SDK is installed: `dotnet --version`
- Check if port 5000 is already in use
- Delete `bin` and `obj` folders, then run `dotnet restore`

### Dashboard won't start
- Make sure Node.js is installed: `node --version`
- Delete `node_modules` folder and run `npm install` again
- Check if port 5173 is already in use

### Can't log in
- Make sure the backend API is running
- Check the browser console for errors (F12)
- Verify you're using the correct username/password

### Sensor simulator errors
- Make sure Python 3 is installed: `python3 --version`
- Install requests library: `pip3 install requests`
- Make sure the API is running on http://localhost:5000

---

## 👨‍💻 Author

**Alex**  
Dundalk Institute of Technology  
Service Oriented Architecture - CA2  
December 2025

---

## 📄 License

This project is for educational purposes as part of DkIT coursework.

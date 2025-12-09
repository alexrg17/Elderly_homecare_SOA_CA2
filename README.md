# Elderly Care Home Room Monitoring API

A comprehensive REST API for managing elderly care home operations, including room management, resident tracking, environmental sensor monitoring, and alert systems.

## 📋 Project Overview

This API service implements a Service-Oriented Architecture (SOA) for monitoring and managing elderly care facilities. It provides endpoints for managing residents, rooms, environmental sensors (temperature/humidity), and alerts for caretakers and administrators.

**Student:** [Your Name]  
**Course:** Service Oriented Architecture (SOA)  
**Assignment:** CA2 - Individual Web Service  
**Institution:** Dundalk Institute of Technology

---

## 🎯 Features

### Core Functionality
- ✅ **Complete CRUD Operations** for 5 entities (Users, Rooms, Residents, SensorData, Alerts)
- ✅ **JWT Authentication & Authorization** with role-based access control (Admin, Caretaker, Viewer)
- ✅ **Entity Framework Core** with SQL Server database
- ✅ **Repository Pattern** for separation of concerns
- ✅ **DTOs** to separate data models from API responses
- ✅ **One-to-Many Relationships**:
  - Room → Residents
  - Room → SensorData
  - Room → Alerts

### Additional Features
- 🔐 Secure password hashing with BCrypt
- 📊 RESTful API design following best practices
- 📝 Comprehensive Swagger/OpenAPI documentation
- 🏗️ Clean Architecture with proper separation of concerns
- ⚡ Async/await for improved performance
- 🎨 Role-based authorization

---

## 🏗️ Architecture

### Project Structure
```
CA2_SOA/
├── Controllers/          # API Controllers (REST endpoints)
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── RoomsController.cs
│   ├── ResidentsController.cs
│   ├── SensorDataController.cs
│   └── AlertsController.cs
├── Models/              # Entity Models (Database tables)
│   ├── User.cs
│   ├── Room.cs
│   ├── Resident.cs
│   ├── SensorData.cs
│   └── Alert.cs
├── DTOs/               # Data Transfer Objects
│   ├── UserDTOs.cs
│   ├── RoomDTOs.cs
│   ├── ResidentDTOs.cs
│   ├── SensorDataDTOs.cs
│   └── AlertDTOs.cs
├── Data/               # Database Context
│   └── CareHomeDbContext.cs
├── Interfaces/         # Repository Interfaces
│   ├── IRepository.cs
│   ├── IUserRepository.cs
│   ├── IRoomRepository.cs
│   ├── IResidentRepository.cs
│   ├── ISensorDataRepository.cs
│   └── IAlertRepository.cs
├── Repositories/       # Repository Implementations
│   ├── UserRepository.cs
│   ├── RoomRepository.cs
│   ├── ResidentRepository.cs
│   ├── SensorDataRepository.cs
│   └── AlertRepository.cs
├── Services/           # Business Logic Services
│   ├── JwtService.cs
│   └── AuthService.cs
└── Program.cs          # Application Entry Point
```

### Design Patterns Used
- **Repository Pattern**: Abstraction layer for data access
- **Dependency Injection**: For loose coupling and testability
- **DTO Pattern**: Separation between domain models and API contracts
- **Service Layer**: Business logic separation

---

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022, Rider, or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <your-repo-url>
   cd CA2_SOA
   ```

2. **Update Database Connection String**
   
   Edit `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CareHomeDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

3. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

4. **Run the Application**
   ```bash
   dotnet run --project CA2_SOA
   ```

5. **Access Swagger UI**
   
   Navigate to: `https://localhost:5001` or `http://localhost:5000`

---

## 🔐 Authentication

### Default Credentials

**Admin Account:**
- Username: `admin`
- Password: `Admin123!`

**Caretaker Account:**
- Username: `caretaker1`
- Password: `Care123!`

### How to Authenticate

1. **Login via API**
   ```
   POST /api/auth/login
   Body: {
     "username": "admin",
     "password": "Admin123!"
   }
   ```

2. **Copy the JWT token** from the response

3. **In Swagger UI:**
   - Click "Authorize" button
   - Enter: `Bearer <your-token-here>`
   - Click "Authorize"

4. **All subsequent requests** will include the token

---

## 📡 API Endpoints

### Authentication
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/register` - Register new user

### Users (🔒 Requires Auth)
- `GET /api/users` - Get all users (Admin only)
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user (Admin only)

### Rooms (🔒 Requires Auth)
- `GET /api/rooms` - Get all rooms
- `GET /api/rooms/{id}` - Get room by ID
- `GET /api/rooms/{id}/details` - Get room with residents, sensors, alerts
- `GET /api/rooms/occupied` - Get occupied rooms
- `GET /api/rooms/available` - Get available rooms
- `POST /api/rooms` - Create new room (Admin only)
- `PUT /api/rooms/{id}` - Update room
- `DELETE /api/rooms/{id}` - Delete room (Admin only)

### Residents (🔒 Requires Auth)
- `GET /api/residents` - Get all residents
- `GET /api/residents/active` - Get active residents
- `GET /api/residents/{id}` - Get resident by ID
- `GET /api/residents/room/{roomId}` - Get residents by room
- `POST /api/residents` - Create new resident
- `PUT /api/residents/{id}` - Update resident
- `DELETE /api/residents/{id}` - Delete resident (Admin only)

### Sensor Data (🔒 Requires Auth)
- `GET /api/sensordata` - Get all sensor readings
- `GET /api/sensordata/recent?count=50` - Get recent readings
- `GET /api/sensordata/{id}` - Get reading by ID
- `GET /api/sensordata/room/{roomId}` - Get readings by room
- `GET /api/sensordata/room/{roomId}/latest` - Get latest reading for room
- `GET /api/sensordata/daterange?startDate=...&endDate=...` - Get by date range
- `POST /api/sensordata` - Create new reading (IoT devices)
- `PUT /api/sensordata/{id}` - Update reading (Admin only)
- `DELETE /api/sensordata/{id}` - Delete reading (Admin only)

### Alerts (🔒 Requires Auth)
- `GET /api/alerts` - Get all alerts
- `GET /api/alerts/active` - Get unresolved alerts
- `GET /api/alerts/{id}` - Get alert by ID
- `GET /api/alerts/room/{roomId}` - Get alerts by room
- `GET /api/alerts/severity/{severity}` - Get by severity (Low, Medium, High, Critical)
- `POST /api/alerts` - Create new alert
- `POST /api/alerts/{id}/resolve` - Resolve alert
- `PUT /api/alerts/{id}` - Update alert
- `DELETE /api/alerts/{id}` - Delete alert (Admin only)

---

## 💾 Database Schema

### Tables & Relationships

**Users**
- Primary key: Id
- Fields: Username, PasswordHash, FullName, Email, Role, CreatedAt, IsActive

**Rooms**
- Primary key: Id
- Fields: RoomNumber (unique), RoomName, Floor, Capacity, IsOccupied, Notes, CreatedAt
- **Has Many**: Residents, SensorReadings, Alerts

**Residents**
- Primary key: Id
- Foreign key: RoomId (nullable)
- Fields: FirstName, LastName, DateOfBirth, MedicalConditions, EmergencyContact, EmergencyPhone, AdmissionDate, IsActive
- **Belongs To**: Room

**SensorData**
- Primary key: Id
- Foreign key: RoomId (required)
- Fields: Temperature, Humidity, Timestamp, SensorType, Notes
- **Belongs To**: Room

**Alerts**
- Primary key: Id
- Foreign key: RoomId (required)
- Fields: AlertType, Severity, Message, CreatedAt, IsResolved, ResolvedAt, ResolvedByUserId, ResolutionNotes
- **Belongs To**: Room

---

## 🧪 Testing with Postman

### Import Collection Steps

1. Create new Postman collection: "Care Home API"
2. Set collection variable: `baseUrl = https://localhost:5001`
3. Set collection variable: `token` (will be filled after login)

### Example Requests

**1. Login**
```
POST {{baseUrl}}/api/auth/login
Body (JSON):
{
  "username": "admin",
  "password": "Admin123!"
}
```
→ Save the `token` from response

**2. Get All Rooms**
```
GET {{baseUrl}}/api/rooms
Headers:
  Authorization: Bearer {{token}}
```

**3. Create Sensor Reading**
```
POST {{baseUrl}}/api/sensordata
Headers:
  Authorization: Bearer {{token}}
Body (JSON):
{
  "roomId": 1,
  "temperature": 22.5,
  "humidity": 45.0,
  "sensorType": "DHT22"
}
```

**4. Get Room Details**
```
GET {{baseUrl}}/api/rooms/1/details
Headers:
  Authorization: Bearer {{token}}
```

---

## 📚 Technologies & Packages

### Core Technologies
- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core 8.0** - ORM for database access
- **SQL Server** - Relational database

### NuGet Packages
- `Microsoft.EntityFrameworkCore` (8.0.11)
- `Microsoft.EntityFrameworkCore.SqlServer` (8.0.11)
- `Microsoft.EntityFrameworkCore.Tools` (8.0.11)
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.11)
- `System.IdentityModel.Tokens.Jwt` (8.2.1)
- `BCrypt.Net-Next` (4.0.3)
- `Swashbuckle.AspNetCore` (6.6.2)

---

## 📖 Key Concepts & Terms

### SOA Principles Applied
- **Loose Coupling**: Repository interfaces decouple data access from business logic
- **Reusability**: Generic repository pattern allows code reuse
- **Abstraction**: DTOs abstract database entities from API consumers
- **Statelessness**: REST API with JWT for stateless authentication

### ORM (Object-Relational Mapping)
Entity Framework Core maps C# classes to database tables, eliminating need for raw SQL queries.

### Routes
RESTful routes follow convention:
- `GET /api/resource` - List all
- `GET /api/resource/{id}` - Get one
- `POST /api/resource` - Create
- `PUT /api/resource/{id}` - Update
- `DELETE /api/resource/{id}` - Delete

### Idempotent Operations
- `GET`, `PUT`, `DELETE` are idempotent (same result when repeated)
- `POST` is NOT idempotent (creates new resource each time)

---

## 🎓 References & Sources

### Official Documentation
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [JWT Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)

### Third-Party Libraries
- [BCrypt.Net GitHub](https://github.com/BcryptNet/bcrypt.net) - Password hashing
- [Swagger/OpenAPI](https://swagger.io/) - API documentation

### Learning Resources
- Microsoft Learn - ASP.NET Core tutorials
- Stack Overflow - Various troubleshooting solutions (cited in code comments)

---

## 🚀 Deployment

### Local Deployment
The application uses SQL Server LocalDB by default for development.

### Cloud Deployment Options

**Azure:**
1. Create Azure SQL Database
2. Update connection string
3. Deploy to Azure App Service

**AWS:**
1. Create RDS SQL Server instance
2. Deploy to Elastic Beanstalk or EC2

**DKIT Cloud:**
Follow institutional guidelines for deployment to internal infrastructure.

---

## ✅ CA Requirements Checklist

- [x] **Requirement 1:** Minimum 4 CRUD services (30%)
  - Users, Rooms, Residents, SensorData, Alerts - all with full CRUD
  
- [x] **Requirement 2:** Authentication/Identity Management (10%)
  - JWT-based authentication with login and role-based authorization
  
- [x] **Requirement 3:** Persistent Storage with Design Patterns (20%)
  - SQL Server database with EF Core
  - Repository Pattern + Dependency Injection
  - 5 tables with one-to-many relationships
  - DTOs separate from entity models
  
- [x] **Requirement 4:** Deployment (10%)
  - Ready for cloud deployment (Azure/AWS/DKIT)
  
- [x] **Requirement 5:** Testing (10%)
  - Swagger UI for manual testing
  - Postman collection ready
  - Unit tests can be added
  
- [x] **Requirement 6:** Extra Facilities (20%)
  - Can add: Mobile app, real-time dashboard, IoT integration

---

## 👨‍💻 Author

**[Your Name]**  
Student at Dundalk Institute of Technology  
Service Oriented Architecture - CA2

---

## 📄 License

This project is for educational purposes as part of DkIT coursework.


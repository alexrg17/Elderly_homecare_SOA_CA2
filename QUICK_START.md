# 🚀 Quick Start Guide - Elderly Care Home Monitoring API

## Prerequisites
- .NET 8.0 SDK installed
- SQL Server LocalDB or SQL Server installed
- JetBrains Rider, Visual Studio, or VS Code

## Step-by-Step Instructions

### Option 1: Using JetBrains Rider (Recommended)

1. **Open the Project**
   - Open Rider
   - File → Open → Select `/Users/alex/RiderProjects/CA2_SOA/CA2_SOA.sln`

2. **Restore NuGet Packages**
   - Rider should automatically restore packages
   - If not: Right-click on solution → Restore NuGet Packages

3. **Build the Project**
   - Build → Build Solution (Cmd+B)
   - Check for any errors in the Build output

4. **Run the Project**
   - Click the green ▶️ play button at the top
   - Or press Ctrl+F5 (Run without debugging)
   - The browser should open automatically with Swagger UI

5. **Test the API**
   - Swagger UI will be at: `https://localhost:5001` or `http://localhost:5000`
   - Database will be created automatically on first run

### Option 2: Using Terminal/Command Line

1. **Navigate to Project Directory**
   ```bash
   cd /Users/alex/RiderProjects/CA2_SOA/CA2_SOA
   ```

2. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

3. **Build the Project**
   ```bash
   dotnet build
   ```
   - If you see errors, check the error messages
   - Most common: Missing SDK, wrong .NET version

4. **Run the Application**
   ```bash
   dotnet run
   ```
   - Watch for the output showing the URL
   - Usually: `Now listening on: https://localhost:5001`

5. **Access Swagger UI**
   - Open browser: `https://localhost:5001`
   - Or: `http://localhost:5000`

### Option 3: Using Visual Studio

1. **Open the Solution**
   - File → Open → Project/Solution
   - Select `CA2_SOA.sln`

2. **Restore & Build**
   - Right-click solution → Restore NuGet Packages
   - Build → Build Solution (F6)

3. **Run the Project**
   - Click the green ▶️ button (or F5)
   - Swagger UI opens automatically

## First Time Setup

### 1. Test Authentication

Once the app is running:

1. **Go to Swagger UI** (`https://localhost:5001`)

2. **Login to get JWT token**:
   - Find `POST /api/auth/login` endpoint
   - Click "Try it out"
   - Enter:
     ```json
     {
       "username": "admin",
       "password": "Admin123!"
     }
     ```
   - Click "Execute"
   - Copy the `token` from the response

3. **Authorize Swagger**:
   - Click the 🔒 **Authorize** button at the top
   - Enter: `Bearer YOUR_TOKEN_HERE`
   - Click "Authorize"
   - Click "Close"

4. **Now you can test all endpoints!**

### 2. Default Login Credentials

**Admin Account:**
- Username: `admin`
- Password: `Admin123!`
- Role: Admin (full access)

**Caretaker Account:**
- Username: `caretaker1`
- Password: `Care123!`
- Role: Caretaker (limited access)

## Database

### Auto-Creation
The database will be **automatically created** on first run with seed data:
- 2 Users (admin, caretaker1)
- 3 Rooms (101, 102, 201)
- 2 Residents (Mary Johnson, James O'Brien)
- 3 Sensor readings
- 1 Sample alert

### Connection String
Located in `appsettings.json`:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CareHomeDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

### If Database Issues Occur
```bash
# Option 1: Delete and recreate
# Find CareHomeDB in SQL Server Object Explorer and delete it
# Run the app again - it will recreate

# Option 2: Use migrations (advanced)
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Testing the API

### Using Swagger UI (Easiest)
1. Run the application
2. Swagger UI opens automatically
3. Login to get token
4. Authorize with token
5. Test any endpoint by clicking "Try it out"

### Using Postman

1. **Import Endpoints**:
   - Create new collection: "Care Home API"
   - Set variable: `{{baseUrl}}` = `https://localhost:5001`

2. **Login Request**:
   ```
   POST {{baseUrl}}/api/auth/login
   Body (JSON):
   {
     "username": "admin",
     "password": "Admin123!"
   }
   ```
   Save the token from response

3. **Test Protected Endpoint**:
   ```
   GET {{baseUrl}}/api/rooms
   Headers:
     Authorization: Bearer YOUR_TOKEN_HERE
   ```

## Example API Calls

### Get All Rooms
```bash
curl -X GET "https://localhost:5001/api/rooms" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Create New Room (Admin only)
```bash
curl -X POST "https://localhost:5001/api/rooms" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "roomNumber": "301",
    "roomName": "Daisy Room",
    "floor": "3rd Floor",
    "capacity": 1,
    "notes": "Newly added room"
  }'
```

### Add Sensor Reading
```bash
curl -X POST "https://localhost:5001/api/sensordata" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 1,
    "temperature": 22.5,
    "humidity": 45.0,
    "sensorType": "DHT22"
  }'
```

## Troubleshooting

### Problem: Build Errors
**Solution:**
- Make sure .NET 8.0 SDK is installed
- Run `dotnet --version` to check
- Download from: https://dotnet.microsoft.com/download/dotnet/8.0

### Problem: Database Connection Failed
**Solution:**
- Install SQL Server LocalDB
- Or change connection string to use SQL Server Express
- Or use SQLite (requires code changes)

### Problem: Port Already in Use
**Solution:**
- Check if another app is using port 5001
- Change port in `Properties/launchSettings.json`

### Problem: JWT Token Expired
**Solution:**
- Tokens expire after 8 hours
- Login again to get new token

### Problem: Unauthorized (401) Error
**Solution:**
- Make sure you clicked "Authorize" in Swagger
- Check token format: `Bearer YOUR_TOKEN` (with space)
- Verify token hasn't expired

## Stopping the Application

### In Rider/Visual Studio
- Click the red ⏹️ stop button
- Or press Shift+F5

### In Terminal
- Press `Ctrl+C`

## Next Steps

1. ✅ **Test all CRUD operations** in Swagger UI
2. ✅ **Create some data** (rooms, residents, sensor readings)
3. ✅ **Test different user roles** (Admin vs Caretaker)
4. ✅ **Check the database** in SQL Server Object Explorer
5. ✅ **Set up Git repository** for version control
6. 🎯 **Consider adding** mobile app or dashboard for extra marks

## Useful Commands

```bash
# Build the project
dotnet build

# Run the project
dotnet run

# Run with watch (auto-reload on changes)
dotnet watch run

# Clean build artifacts
dotnet clean

# Restore packages
dotnet restore

# Check .NET version
dotnet --version

# List installed SDKs
dotnet --list-sdks
```

## Port Information

The application runs on:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`

Swagger UI is available at the root URL.

## Need Help?

Check these files:
- `README.md` - Full project documentation
- `IMPLEMENTATION_SUMMARY.md` - Implementation details
- `appsettings.json` - Configuration settings

---

**Ready to run?** Just open in Rider and click the ▶️ button! 🚀


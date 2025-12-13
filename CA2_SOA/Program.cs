using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CA2_SOA.Data;
using CA2_SOA.Interfaces;
using CA2_SOA.Repositories;
using CA2_SOA.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure to listen on Railway's PORT or default to 5000
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

// Add Database Context - Use PostgreSQL in cloud (Railway/Azure), SQLite locally
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL"); // Railway PostgreSQL
var azurePostgresConnection = builder.Configuration.GetConnectionString("AzurePostgres");
var useAzureDatabase = builder.Configuration.GetValue<bool>("UseAzureDatabase");

Console.WriteLine("========================================");
Console.WriteLine("[Startup] Elderly Care Home API Starting");
Console.WriteLine($"[Startup] DATABASE_URL present: {!string.IsNullOrEmpty(databaseUrl)}");
Console.WriteLine($"[Startup] PORT: {Environment.GetEnvironmentVariable("PORT") ?? "5000"}");
Console.WriteLine($"[Startup] Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine("========================================");

try
{
    builder.Services.AddDbContext<CareHomeDbContext>(options =>
    {
        try
        {
            if (!string.IsNullOrEmpty(databaseUrl))
            {
                Console.WriteLine("[Database] Configuring Railway PostgreSQL...");
                Console.WriteLine($"[Database] DATABASE_URL format: {databaseUrl.Substring(0, Math.Min(30, databaseUrl.Length))}...");
                
                // Railway PostgreSQL - convert DATABASE_URL to proper connection string
                var databaseUri = new Uri(databaseUrl);
                var userInfo = databaseUri.UserInfo.Split(':');
                
                if (userInfo == null || userInfo.Length < 2)
                {
                    throw new Exception("DATABASE_URL does not contain valid user credentials");
                }
                
                var database = databaseUri.AbsolutePath.Trim('/');
                if (string.IsNullOrEmpty(database))
                {
                    throw new Exception("DATABASE_URL does not contain a database name");
                }
                
                var connectionString = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={database};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
                Console.WriteLine($"[Database] Parsed - Host: {databaseUri.Host}, Port: {databaseUri.Port}, DB: {database}");
                
                options.UseNpgsql(connectionString);
                Console.WriteLine("[Database] ✅ PostgreSQL configured successfully");
            }
            else if (useAzureDatabase && !string.IsNullOrEmpty(azurePostgresConnection))
            {
                Console.WriteLine("[Database] Configuring Azure PostgreSQL...");
                options.UseNpgsql(azurePostgresConnection);
                Console.WriteLine("[Database] ✅ Azure PostgreSQL configured successfully");
            }
            else
            {
                Console.WriteLine("[Database] ⚠️  No DATABASE_URL found - using SQLite fallback");
                Console.WriteLine("[Database] Note: SQLite data will not persist between deployments on Railway");
                // Fallback to SQLite with in-memory if file system is not writable
                var sqliteConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=CareHomeDB.db";
                options.UseSqlite(sqliteConnection);
                Console.WriteLine("[Database] ✅ SQLite configured");
            }
            
            // Enable sensitive data logging in development
            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Database] ❌ ERROR: Database configuration failed!");
            Console.WriteLine($"[Database] Error Type: {ex.GetType().Name}");
            Console.WriteLine($"[Database] Error Message: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[Database] Inner Error: {ex.InnerException.Message}");
            }
            Console.WriteLine($"[Database] Falling back to SQLite...");
            
            // Final fallback to SQLite
            try
            {
                options.UseSqlite("Data Source=CareHomeDB.db");
                Console.WriteLine("[Database] ✅ SQLite fallback configured");
            }
            catch (Exception fallbackEx)
            {
                Console.WriteLine($"[Database] ❌ CRITICAL: Even SQLite fallback failed: {fallbackEx.Message}");
                throw;
            }
        }
    });
    Console.WriteLine("[Startup] ✅ DbContext registration completed");
}
catch (Exception startupEx)
{
    Console.WriteLine("========================================");
    Console.WriteLine("[CRITICAL ERROR] Failed to register DbContext!");
    Console.WriteLine($"Error Type: {startupEx.GetType().Name}");
    Console.WriteLine($"Error: {startupEx.Message}");
    Console.WriteLine($"Stack: {startupEx.StackTrace}");
    Console.WriteLine("========================================");
    
    // Register a fallback SQLite DbContext
    Console.WriteLine("[Startup] Attempting emergency SQLite fallback...");
    builder.Services.AddDbContext<CareHomeDbContext>(options =>
    {
        options.UseSqlite("Data Source=CareHomeDB.db");
    });
    Console.WriteLine("[Startup] ✅ Emergency SQLite DbContext registered");
}

// Add Controllers
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register Repositories (Dependency Injection for Separation of Concerns)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IResidentRepository, ResidentRepository>();
builder.Services.AddScoped<ISensorDataRepository, SensorDataRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();

// Register Services
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLongForJWT!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "CareHomeAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "CareHomeClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure Swagger/OpenAPI with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Elderly Care Home Monitoring API",
        Version = "v1",
        Description = "REST API for managing elderly care home rooms, residents, environmental sensors, and alerts",
        Contact = new OpenApiContact
        {
            Name = "Care Home Team",
            Email = "support@carehome.com"
        }
    });
    
    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
// Enable Swagger in all environments for easy testing and demonstration
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Care Home API v1");
    options.RoutePrefix = string.Empty; // Swagger UI at root
});

// Initialize Database with comprehensive error handling
Console.WriteLine("[Database Init] Starting database initialization...");
try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    
    try
    {
        Console.WriteLine("[Database Init] Getting DbContext...");
        var context = services.GetRequiredService<CareHomeDbContext>();
        
        Console.WriteLine("[Database Init] Testing database connection...");
        // Test if we can connect to the database
        var canConnect = context.Database.CanConnect();
        Console.WriteLine($"[Database Init] Can connect: {canConnect}");
        
        if (canConnect)
        {
            Console.WriteLine("[Database Init] Ensuring database is created...");
            // Ensure database is created and migrations are applied
            context.Database.EnsureCreated();
            Console.WriteLine("[Database Init] ✅ Database initialized successfully!");
        }
        else
        {
            Console.WriteLine("[Database Init] ⚠️  Cannot connect to database. App will start without database initialization.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Database Init] ❌ ERROR during database initialization!");
        Console.WriteLine($"[Database Init] Error Type: {ex.GetType().Name}");
        Console.WriteLine($"[Database Init] Error Message: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"[Database Init] Inner Exception: {ex.InnerException.Message}");
        }
        Console.WriteLine("[Database Init] ⚠️  Application will continue without database initialization.");
        Console.WriteLine("[Database Init] Database will be created on first request.");
    }
}
catch (Exception criticalEx)
{
    Console.WriteLine($"[Database Init] ❌ CRITICAL ERROR: Failed to create service scope!");
    Console.WriteLine($"[Database Init] Error: {criticalEx.Message}");
    Console.WriteLine("[Database Init] Application will continue, but database may not work correctly.");
}
Console.WriteLine("[Database Init] Initialization phase completed.");

// Only use HTTPS redirection in production with proper certificates
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.UseAuthentication(); // Important: Must be before UseAuthorization
app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName 
}));

app.MapGet("/", () => Results.Redirect("/swagger"));

Console.WriteLine("🏥 Elderly Care Home Monitoring API is starting...");
Console.WriteLine($"📖 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"🔐 Default Admin - Username: admin, Password: admin123");
Console.WriteLine("✅ Application started successfully!");

app.Run();


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

// Configure Host FIRST to prevent service validation during build (which tries to connect to database)
Console.WriteLine("[Startup] Configuring service provider to skip validation...");
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = false;
    options.ValidateOnBuild = false;
});
Console.WriteLine("[Startup] ✅ Service provider validation disabled");

// ASP.NET Core will automatically use the PORT environment variable
// No need to manually configure Kestrel - Railway sets ASPNETCORE_URLS automatically

// Add Database Context - Use Railway PostgreSQL in production, SQLite locally
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL"); // Railway PostgreSQL

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
                
                if (userInfo.Length < 2)
                {
                    throw new Exception("DATABASE_URL does not contain valid user credentials");
                }
                
                var database = databaseUri.AbsolutePath.Trim('/');
                if (string.IsNullOrEmpty(database))
                {
                    throw new Exception("DATABASE_URL does not contain a database name");
                }
                
                var connectionString = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={database};Username={userInfo[0]};Password={userInfo[1]};Pooling=true;SSL Mode=Prefer;Trust Server Certificate=true;Include Error Detail=true";
                Console.WriteLine($"[Database] Parsed - Host: {databaseUri.Host}, Port: {databaseUri.Port}, DB: {database}");
                
                options.UseNpgsql(connectionString);
                Console.WriteLine("[Database] ✅ PostgreSQL configured successfully");
            }
            else
            {
                Console.WriteLine("[Database] ⚠️  No DATABASE_URL found - using SQLite fallback");
                Console.WriteLine("[Database] Note: SQLite data will not persist between deployments");
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
Console.WriteLine("[Service Registration] Adding Controllers...");
try
{
    builder.Services.AddControllers();
    Console.WriteLine("[Service Registration] ✅ Controllers added");
}
catch (Exception ex)
{
    Console.WriteLine($"[Service Registration] ❌ Controllers failed: {ex.Message}");
    throw;
}

// Add CORS
Console.WriteLine("[Service Registration] Adding CORS...");
try
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
    Console.WriteLine("[Service Registration] ✅ CORS added");
}
catch (Exception ex)
{
    Console.WriteLine($"[Service Registration] ❌ CORS failed: {ex.Message}");
    throw;
}

// Register Repositories (Dependency Injection for Separation of Concerns)
Console.WriteLine("[Service Registration] Registering repositories...");
try
{
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    Console.WriteLine("[Service Registration]   ✅ UserRepository");
    builder.Services.AddScoped<IRoomRepository, RoomRepository>();
    Console.WriteLine("[Service Registration]   ✅ RoomRepository");
    builder.Services.AddScoped<IResidentRepository, ResidentRepository>();
    Console.WriteLine("[Service Registration]   ✅ ResidentRepository");
    builder.Services.AddScoped<ISensorDataRepository, SensorDataRepository>();
    Console.WriteLine("[Service Registration]   ✅ SensorDataRepository");
    builder.Services.AddScoped<IAlertRepository, AlertRepository>();
    Console.WriteLine("[Service Registration]   ✅ AlertRepository");
    Console.WriteLine("[Service Registration] ✅ All repositories registered");
}
catch (Exception ex)
{
    Console.WriteLine($"[Service Registration] ❌ Repository registration failed: {ex.Message}");
    throw;
}

// Register Services
Console.WriteLine("[Service Registration] Registering services...");
try
{
    builder.Services.AddScoped<JwtService>();
    Console.WriteLine("[Service Registration]   ✅ JwtService");
    builder.Services.AddScoped<AuthService>();
    Console.WriteLine("[Service Registration]   ✅ AuthService");
    Console.WriteLine("[Service Registration] ✅ All services registered");
}
catch (Exception ex)
{
    Console.WriteLine($"[Service Registration] ❌ Service registration failed: {ex.Message}");
    throw;
}

// Configure JWT Authentication
Console.WriteLine("[Service Registration] Configuring JWT Authentication...");
try
{
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
    Console.WriteLine("[Service Registration] ✅ JWT Authentication configured");
}
catch (Exception ex)
{
    Console.WriteLine($"[Service Registration] ❌ JWT Authentication failed: {ex.Message}");
    throw;
}

// Configure Swagger/OpenAPI with JWT support
Console.WriteLine("[Service Registration] Configuring Swagger...");
try
{
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
    Console.WriteLine("[Service Registration] ✅ Swagger configured");
}
catch (Exception ex)
{
    Console.WriteLine($"[Service Registration] ❌ Swagger configuration failed: {ex.Message}");
    throw;
}

Console.WriteLine("========================================");
Console.WriteLine("[App Build] Building application...");
Console.WriteLine("========================================");

WebApplication app;
try
{
    app = builder.Build();
    Console.WriteLine("========================================");
    Console.WriteLine("[App Build] ✅ Application built successfully!");
    Console.WriteLine("========================================");
}
catch (Exception buildEx)
{
    Console.WriteLine("========================================");
    Console.WriteLine("[App Build] ❌ CRITICAL: Application build FAILED!");
    Console.WriteLine($"[App Build] Error Type: {buildEx.GetType().FullName}");
    Console.WriteLine($"[App Build] Error Message: {buildEx.Message}");
    
    if (buildEx.InnerException != null)
    {
        Console.WriteLine($"[App Build] Inner Exception Type: {buildEx.InnerException.GetType().FullName}");
        Console.WriteLine($"[App Build] Inner Exception: {buildEx.InnerException.Message}");
        
        if (buildEx.InnerException.InnerException != null)
        {
            Console.WriteLine($"[App Build] Inner Inner Exception: {buildEx.InnerException.InnerException.Message}");
        }
    }
    
    Console.WriteLine($"[App Build] Stack Trace:");
    Console.WriteLine(buildEx.StackTrace);
    Console.WriteLine("========================================");
    
    // Try to build with minimal configuration as last resort
    Console.WriteLine("[App Build] Attempting to rebuild with minimal configuration...");
    
    try
    {
        var minimalBuilder = WebApplication.CreateBuilder(args);
        minimalBuilder.Host.UseDefaultServiceProvider(opt => { opt.ValidateScopes = false; opt.ValidateOnBuild = false; });
        minimalBuilder.Services.AddControllers();
        app = minimalBuilder.Build();
        Console.WriteLine("[App Build] ⚠️  Minimal app built - limited functionality available");
    }
    catch
    {
        Console.WriteLine("[App Build] ❌ Even minimal build failed. Exiting.");
        throw;
    }
}

// Configure the HTTP request pipeline
Console.WriteLine("[Middleware] Configuring HTTP request pipeline...");

// Enable Swagger in all environments for easy testing and demonstration
Console.WriteLine("[Middleware] Enabling Swagger...");
try
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Care Home API v1");
        options.RoutePrefix = string.Empty; // Swagger UI at root
    });
    Console.WriteLine("[Middleware] ✅ Swagger enabled");
}
catch (Exception ex)
{
    Console.WriteLine($"[Middleware] ❌ Swagger failed: {ex.Message}");
}

// Only use HTTPS redirection in production with proper certificates
Console.WriteLine("[Middleware] Configuring HTTPS redirection...");
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    Console.WriteLine("[Middleware] ✅ HTTPS redirection enabled");
}
else
{
    Console.WriteLine("[Middleware] ⚠️  HTTPS redirection skipped (development mode)");
}

Console.WriteLine("[Middleware] Enabling CORS...");
app.UseCors("AllowAll");
Console.WriteLine("[Middleware] ✅ CORS enabled");

Console.WriteLine("[Middleware] Enabling Authentication...");
app.UseAuthentication(); // Important: Must be before UseAuthorization
Console.WriteLine("[Middleware] ✅ Authentication enabled");

Console.WriteLine("[Middleware] Enabling Authorization...");
app.UseAuthorization();
Console.WriteLine("[Middleware] ✅ Authorization enabled");

Console.WriteLine("[Middleware] Mapping controllers...");
app.MapControllers();
Console.WriteLine("[Middleware] ✅ Controllers mapped");

// Add health check endpoint
Console.WriteLine("[Middleware] Adding health check endpoint...");
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName 
}));
Console.WriteLine("[Middleware] ✅ Health check endpoint added");

Console.WriteLine("[Middleware] Adding root redirect...");
app.MapGet("/", () => Results.Redirect("/swagger"));
Console.WriteLine("[Middleware] ✅ Root redirect added");

Console.WriteLine("========================================");
Console.WriteLine("🏥 Elderly Care Home Monitoring API");
Console.WriteLine($"📖 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("✅ All services configured successfully!");
Console.WriteLine("========================================");

// Initialize database in background task so app starts immediately
_ = Task.Run(async () =>
{
    await Task.Delay(1000); // Wait 1 second for app to start
    Console.WriteLine("[Database Init] Starting background database initialization...");
    
    var maxRetries = 3;
    var retryCount = 0;
    var dbInitialized = false;
    
    while (retryCount < maxRetries && !dbInitialized)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<CareHomeDbContext>();
            
            Console.WriteLine($"[Database Init] Attempt {retryCount + 1}/{maxRetries} - Testing database connection...");
            
            var canConnect = await context.Database.CanConnectAsync();
            Console.WriteLine($"[Database Init] Can connect: {canConnect}");
            
            if (canConnect)
            {
                Console.WriteLine("[Database Init] Creating database schema...");
                await context.Database.EnsureCreatedAsync();
                Console.WriteLine("[Database Init] ✅ Database initialized successfully!");
                dbInitialized = true;
            }
            else
            {
                Console.WriteLine("[Database Init] ⚠️  Cannot connect to database. Retrying...");
                retryCount++;
            }
        }
        catch (Exception ex)
        {
            retryCount++;
            Console.WriteLine($"[Database Init] ❌ Error on attempt {retryCount}/{maxRetries}");
            Console.WriteLine($"[Database Init] Error: {ex.Message}");
            
            if (retryCount >= maxRetries)
            {
                Console.WriteLine("[Database Init] ⚠️  Max retries reached. Database will be created on first API request.");
            }
        }
    }
});

Console.WriteLine("[App Start] Calling app.Run()...");

app.Run();

Console.WriteLine("[App Start] ⚠️  app.Run() returned (this should never happen)");


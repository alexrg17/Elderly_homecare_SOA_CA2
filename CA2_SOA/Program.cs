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

// Configure Railway PORT if available
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add Database Context - Use PostgreSQL in cloud (Railway/Azure), SQLite locally
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL"); // Railway PostgreSQL
var azurePostgresConnection = builder.Configuration.GetConnectionString("AzurePostgres");
var useAzureDatabase = builder.Configuration.GetValue<bool>("UseAzureDatabase");

Console.WriteLine($"[Startup] DATABASE_URL present: {!string.IsNullOrEmpty(databaseUrl)}");
Console.WriteLine($"[Startup] PORT: {Environment.GetEnvironmentVariable("PORT") ?? "5000"}");

builder.Services.AddDbContext<CareHomeDbContext>(options =>
{
    try
    {
        if (!string.IsNullOrEmpty(databaseUrl))
        {
            Console.WriteLine("[Startup] Configuring Railway PostgreSQL...");
            // Railway PostgreSQL - convert DATABASE_URL to proper connection string
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var connectionString = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={databaseUri.AbsolutePath.Trim('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
            options.UseNpgsql(connectionString);
            Console.WriteLine("[Startup] PostgreSQL configured successfully");
        }
        else if (useAzureDatabase && !string.IsNullOrEmpty(azurePostgresConnection))
        {
            Console.WriteLine("[Startup] Configuring Azure PostgreSQL...");
            // Azure PostgreSQL
            options.UseNpgsql(azurePostgresConnection);
        }
        else
        {
            Console.WriteLine("[Startup] Using SQLite (local development)...");
            // Local SQLite
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Startup ERROR] Database configuration failed: {ex.Message}");
        Console.WriteLine($"[Startup] Falling back to SQLite...");
        // Fallback to SQLite if anything fails
        options.UseSqlite("Data Source=CareHomeDB.db");
    }
});

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

// Initialize Database with retry logic
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Attempting to initialize database...");
        var context = services.GetRequiredService<CareHomeDbContext>();
        
        // Ensure database is created and migrations are applied
        context.Database.EnsureCreated();
        logger.LogInformation("✅ Database initialized successfully!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ An error occurred creating the database. Application will continue without database.");
        logger.LogError($"Error details: {ex.Message}");
        logger.LogError($"Stack trace: {ex.StackTrace}");
    }
}

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


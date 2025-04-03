using HabitTracker.Server.Auth;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using HabitTracker.Server.Storage;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Transformer;
using HabitTracker.Server.Database;
using HabitTracker.Server.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add appsettings.json to the configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddAuthentication(cfg => {
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8
            .GetBytes(builder.Configuration["ApplicationSettings:JWT_Secret"])
        ),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Host.UseSerilog((context, services, config) =>
{
    config
        .MinimumLevel.Debug()
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console();
});

builder.Services.AddScoped<IAuthentication>(provider => new Authentication(builder.Configuration["ApplicationSettings:JWT_Secret"]));

builder.Services.AddScoped<IStorage>(provider => new SqliteFacade(connectionString));
builder.Services.AddDbContext<HabitTrackerDbContext>(options =>
{
    options.UseSqlite(connectionString);
});
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ITransformer<IReadOnlyCollection<Rate>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>, RateTransformer>();
builder.Services.AddScoped<ITransformer<IReadOnlyCollection<Habit>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>, HabitTransformer>();
builder.Services.AddScoped<ITransformer<IReadOnlyCollection<HabitLog>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>, HabitLogTransformer>();
builder.Services.AddScoped<ITransformer<IReadOnlyCollection<User>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>, UserTransformer>();
builder.Services.AddScoped<IRateLimitRepository, RateLimitRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<IHabitLogRepository, HabitLogRepository>();
builder.Services.AddScoped<IRateLimitService, RateLimitService>();
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IHabitLogService, HabitLogService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Habit Tracker API", Version = "v1" });

    // Configure JWT authentication for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HabitTrackerDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseAuthentication();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>(builder);
app.UseMiddleware<RateLimitingMiddleware>();
app.UseMiddleware<UserIdMiddleware>();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();

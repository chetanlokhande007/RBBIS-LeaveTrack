using System.Text;
using LeaveAttendance.API.Data;
using LeaveAttendance.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register Services and Repositories
builder.Services.AddScoped<LeaveAttendance.API.Services.Interfaces.IEmailService, LeaveAttendance.API.Services.EmailService>();
builder.Services.AddScoped<LeaveAttendance.API.Services.IJwtService, LeaveAttendance.API.Services.JwtService>();
builder.Services.AddScoped<LeaveAttendance.API.Services.Interfaces.IAuthService, LeaveAttendance.API.Services.AuthService>();
builder.Services.AddScoped<LeaveAttendance.API.Repositories.Interfaces.IEmployeeRepository, LeaveAttendance.API.Repositories.EmployeeRepository>();
builder.Services.AddScoped<LeaveAttendance.API.Services.Interfaces.IEmployeeService, LeaveAttendance.API.Services.EmployeeService>();
builder.Services.AddScoped<LeaveAttendance.API.Repositories.Interfaces.IAttendanceRepository, LeaveAttendance.API.Repositories.AttendanceRepository>();
builder.Services.AddScoped<LeaveAttendance.API.Services.Interfaces.IAttendanceService, LeaveAttendance.API.Services.AttendanceService>();
builder.Services.AddScoped<LeaveAttendance.API.Repositories.Interfaces.ILeaveTypeRepository, LeaveAttendance.API.Repositories.LeaveTypeRepository>();
builder.Services.AddScoped<LeaveAttendance.API.Services.Interfaces.ILeaveTypeService, LeaveAttendance.API.Services.LeaveTypeService>();
builder.Services.AddScoped<LeaveAttendance.API.Repositories.Interfaces.ILeaveRequestRepository, LeaveAttendance.API.Repositories.LeaveRequestRepository>();
builder.Services.AddScoped<LeaveAttendance.API.Services.Interfaces.ILeaveRequestService, LeaveAttendance.API.Services.LeaveRequestService>();
builder.Services.AddScoped<LeaveAttendance.API.Repositories.Interfaces.IHolidayRepository, LeaveAttendance.API.Repositories.HolidayRepository>();
builder.Services.AddScoped<LeaveAttendance.API.Services.Interfaces.IHolidayService, LeaveAttendance.API.Services.HolidayService>();

// Configure Swagger/OpenAPI with JWT Auth Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Leave & Attendance Management API",
        Version = "v1",
        Description = "API endpoints for the Employee Leave & Attendance Management System"
    });
});

// Configure EF Core with PostgreSQL
builder.Services.AddDbContext<LeaveTrackDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<LeaveTrackDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "SuperSecretJWTKeyThatIsAtLeast32CharactersLong!";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular app URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<LeaveAttendance.API.Middleware.GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Leave & Attendance API v1");
        options.RoutePrefix = "swagger"; // Opens swagger UI at http://localhost:<port>/swagger
    });
}

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

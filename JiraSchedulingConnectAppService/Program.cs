using AlgorithmServiceServer.Services.Interfaces;
using JiraSchedulingConnectAppService.Repository;
using JiraSchedulingConnectAppService.Repository.Interfaces;
using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Custom Config
builder.Services.AddCors();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddDbContext<JiraDemoContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("DB")
    )
);



builder.Services.AddSingleton<ILoggerService, DatabaseLoggerService>();


builder.Services.AddHttpContextAccessor();

// Register services

builder.Services.AddTransient<ILogRepository, LogRepository>(_
            => new LogRepository(builder.Configuration.GetConnectionString("DB")));
builder.Services.AddHostedService(sp => sp.GetService<ILoggerService>() as DatabaseLoggerService);


builder.Services.AddTransient<IAPIMicroserviceService, APIMicroserviceService>();
builder.Services.AddTransient<IProjectServices, ProjectsService>();
builder.Services.AddTransient<ISkillsService, SkillsService>();
builder.Services.AddTransient<ITasksService, TasksService>();
builder.Services.AddTransient<IAlgorithmService, AlgorithmService>();
builder.Services.AddTransient<IValidatorService, ScheduleValidatorService>();
//builder.Services.AddTransient<ILoggerService, DatabaseLoggerService>();

builder.Services.AddTransient<IParametersService, ParametersService>();
builder.Services.AddTransient<IWorkforcesService, WorkforcesService>();
builder.Services.AddTransient<IEquipmentService, EquipmentsService>();
builder.Services.AddTransient<IJiraBridgeAPIService, JiraBridgeAPIService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IExportService, ExportService>();
builder.Services.AddTransient<IThreadService, ThreadService>();
builder.Services.AddTransient<IScheduleService, ScheduleService>();
builder.Services.AddTransient<IMilestonesService, MilestonesService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.UseAuthentication();
 app.UseAuthorization();

app.MapControllers();

// Custom Config:
app.UseCors(opt => opt.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.Run();
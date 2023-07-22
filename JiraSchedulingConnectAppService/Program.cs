using AlgorithmServiceServer.Services.Interfaces;
using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using JiraSchedulingConnectAppService.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModelLibrary;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using NLog;
using NLog.Web;
using System.Text;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{

    logger.Info("Start Game...");

    var builder = WebApplication.CreateBuilder(args);

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
    builder.Services.AddSignalR();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Custom Config
    builder.Services.AddCors();
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
    builder.Services.AddDbContext<JiraDemoContext>(opt => opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DB")
        )
    );

    builder.Services.AddHttpContextAccessor();

    // Register services
    builder.Services.AddTransient<ILoggerManager, LoggerManager>();

    builder.Services.AddTransient<IAPIMicroserviceService, APIMicroserviceService>();
    builder.Services.AddTransient<IProjectServices, ProjectsService>();
    builder.Services.AddTransient<ISkillsService, SkillsService>();
    builder.Services.AddTransient<ITasksService, TasksService>();
    builder.Services.AddTransient<IAlgorithmService, AlgorithmService>();
    builder.Services.AddTransient<IValidatorService, ScheduleValidatorService>();

    builder.Services.AddTransient<IParametersService, ParametersService>();
    builder.Services.AddTransient<IWorkforcesService, WorkforcesService>();
    builder.Services.AddTransient<IEquipmentService, EquipmentsService>();
    builder.Services.AddTransient<IJiraBridgeAPIService, JiraBridgeAPIService>();
    builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
    builder.Services.AddTransient<IExportService, ExportService>();
    builder.Services.AddTransient<IThreadService, ThreadService>();
    builder.Services.AddTransient<IScheduleService, ScheduleService>();
    builder.Services.AddTransient<IMilestonesService, MilestonesService>();

    // Config log provider
    builder.Host.ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    }).UseNLog();

    builder.Services.AddLogging();

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
    app.MapHub<SignalRServer>("/signalrServer");

    // Custom Config:
    app.UseCors(opt => opt.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
    await app.RunAsync();

}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}



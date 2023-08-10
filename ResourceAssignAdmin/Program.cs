using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using Quartz;
using ResourceAssignAdmin.Filter;
using ResourceAssignAdmin.Jobs;
using ResourceAssignAdmin.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<WoTaasContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("DB")
    )
);
builder.Services.AddRazorPages().AddRazorPagesOptions(o =>
{
    o.Conventions.AddFolderApplicationModelConvention("/login", model => model.Filters.Add(new SessionFilter()));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);//You can set Time   
});

// Config using Quartz
builder.Services.AddQuartz(q =>
{
    // Configure Quartz options here, if needed
    q.UseMicrosoftDependencyInjectionScopedJobFactory();
    var jobKey = new JobKey("downgradeSubscription");
    q.AddJob<CheckSubscriptionJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("downgradeSubscription-trigger")
        //This Cron interval can be described as "run every minute" (when second is zero)
        .WithCronSchedule("0 * * * * ?")
    );
});
// Add Quartz hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
// --------------

builder.Services.AddTransient<IBraintreeService, BraintreeService>();
builder.Services.AddTransient<ISubscriptionService, SubscriptionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseSession();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

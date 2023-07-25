using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ModelLibrary.DBModels;
using ResourceAssignAdmin.Filter;
using static ModelLibrary.DTOs.Export.MSPXMLModelDTO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<JiraDemoContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("DB")
    )
);
builder.Services.AddRazorPages().AddRazorPagesOptions(o =>
{
    o.Conventions.AddFolderApplicationModelConvention("/", model => model.Filters.Add(new SessionFilter()));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);//You can set Time   
});
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

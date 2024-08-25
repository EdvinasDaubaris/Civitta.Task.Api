using Civitta.Task1.Api.Models;
using Civitta.Task1.Api.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddFastEndpoints();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IEnricoService, EnricoService>();

var connectionString = builder.Configuration.GetConnectionString("main");
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString)) ;

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//app.UseAuthorization();
app.UseFastEndpoints();

app.Run();

using Civitta.Task1.Api.Models;
using Civitta.Task1.Api.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using FastEndpoints.Swagger;
using FastEndpoints.Security;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization().AddFastEndpoints().SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Civitta task";
        s.Version = "v1";
    };
});
builder.Services.AddMemoryCache();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEnricoService, EnricoService>();


var connectionString = builder.Configuration.GetConnectionString("main");
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString)) ;

var app = builder.Build();



app
   .UseAuthorization().UseFastEndpoints().UseSwaggerGen();

app.Run();

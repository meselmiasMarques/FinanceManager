using FinanceManager.Data;
using FinanceManager.Extensions.Endpoints;
using FinanceManager.Repositories;
using FinanceManager.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder
    .Configuration
    .GetConnectionString("DefaultConnection");

builder
    .Services
    .AddDbContext<AppDbContext>(options =>
        options
        .UseNpgsql(connectionString));

//Injeção de dependência dos Repositorios
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();

//Injeção de dependência dos Sevices
builder.Services.AddTransient<ICategoryService, CategoryService>();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddBearerToken();

builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
    });
}








    CategoryEndPoints.MapCategoryEndpoints(app);






app.Run();


using FinanceManager.Data;
using FinanceManager.Extensions;
using FinanceManager.Extensions.Endpoints;
using FinanceManager.Repositories;
using FinanceManager.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string WasmCorsPolicy = "wasm";

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidation();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    // Fallback só para desenvolvimento/design-time (migrations). Configure via user-secrets/appsettings.
    connectionString = "Host=localhost;Port=5432;Database=financemanager;Username=postgres;Password=postgres";
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// Usuário atual (stub enquanto autenticação não é implementada).
builder.Services.AddSingleton<IUserContext, CurrentUserContext>();

// Repositórios
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(WasmCorsPolicy, policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (origins is { Length: > 0 })
            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
        else
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "FinanceManager API v1"));
}

app.UseCors(WasmCorsPolicy);

CategoryEndPoints.Map(app);
TransactionEndPoints.Map(app);
DashboardEndPoints.Map(app);

app.Run();

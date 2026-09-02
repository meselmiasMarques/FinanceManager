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
builder.Services.AddHttpContextAccessor();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    // Fallback só para desenvolvimento/design-time (migrations). Configure via user-secrets/appsettings.
    connectionString = "Host=localhost;Port=5432;Database=financemanager;Username=postgres;Password=postgres";
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// Usuário atual — derivado do token JWT (ver CurrentUserContext).
builder.Services.AddScoped<IUserContext, CurrentUserContext>();

// Autenticação/autorização: Identity + JWT + refresh token + rate limiting.
builder.Services.AddAppAuth(builder.Configuration, builder.Environment);

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
        {
            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        }
        else if (builder.Environment.IsDevelopment())
        {
            // Dev sem origens configuradas: libera qualquer origem, mas com credenciais
            // (necessário para o cookie de refresh) — SetIsOriginAllowed em vez de AllowAnyOrigin.
            policy.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        }
        else
        {
            throw new InvalidOperationException(
                "Cors:AllowedOrigins não configurado. Defina as origens permitidas para produção (DC-04).");
        }
    });
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "FinanceManager API v1"));
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors(WasmCorsPolicy);

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

AuthEndPoints.Map(app);
CategoryEndPoints.Map(app);
TransactionEndPoints.Map(app);
DashboardEndPoints.Map(app);

app.Run();

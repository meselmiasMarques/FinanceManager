using System.Globalization;
using FinanceManager.Web;
using FinanceManager.Web.Services;
using FinanceManager.Web.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// App em português (moeda R$, datas dd/MM/yyyy).
var culture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var apiBaseUrl = builder.Configuration["Api:BaseUrl"]
    ?? throw new InvalidOperationException("Configuração 'Api:BaseUrl' não encontrada em wwwroot/appsettings.json.");

builder.Services.AddMudServices();

// ---- Autenticação / sessão (Etapa 2) --------------------------------------------------
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddSingleton<TokenStore>();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UiPreferences>();

// Cliente de /auth/*: HttpClient puro (sem o handler de refresh), envia credenciais.
builder.Services.AddScoped(sp => new AuthApiClient(
    new HttpClient { BaseAddress = new Uri(apiBaseUrl) },
    sp.GetRequiredService<TokenStore>()));

// Cliente de dados: HttpClient com o AuthMessageHandler (Bearer + refresh no 401).
builder.Services.AddScoped(sp =>
{
    var handler = new AuthMessageHandler(
        sp.GetRequiredService<JwtAuthStateProvider>(),
        sp.GetRequiredService<TokenStore>())
    {
        InnerHandler = new HttpClientHandler()
    };
    return new HttpClient(handler) { BaseAddress = new Uri(apiBaseUrl) };
});

builder.Services.AddScoped<CategoryApiClient>();
builder.Services.AddScoped<TransactionApiClient>();
builder.Services.AddScoped<DashboardApiClient>();

await builder.Build().RunAsync();

using FinanceManager.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FinanceManager.Data
{
    /// <summary>
    /// Usado apenas por <c>dotnet ef</c> (migrations / scripts). Desacopla o design-time
    /// do host web e do <c>HttpContext</c> — o inquilino aqui é sempre "nenhum".
    /// </summary>
    public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddUserSecrets<AppDbContextFactory>(optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Port=5432;Database=financemanager;Username=postgres;Password=postgres";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new AppDbContext(options, new DesignTimeUserContext());
        }

        private sealed class DesignTimeUserContext : IUserContext
        {
            public Guid UserId => throw new InvalidOperationException("Sem usuário em design-time.");
            public Guid? UserIdOrNull => null;
            public bool IsAuthenticated => false;
        }
    }
}

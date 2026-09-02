using System.Reflection;
using FinanceManager.Extensions;
using FinanceManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        private readonly IUserContext _userContext;

        public AppDbContext(DbContextOptions<AppDbContext> options, IUserContext userContext)
            : base(options)
        {
            _userContext = userContext;
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Isolamento multi-tenant (RF-I03): toda leitura já vem filtrada pelo inquilino atual.
            // Anônimo => UserIdOrNull null => nenhuma linha (fail-closed). Os Where(...) manuais
            // nos repositórios permanecem como segunda camada.
            modelBuilder.Entity<Category>().HasQueryFilter(c => c.UserId == _userContext.UserIdOrNull);
            modelBuilder.Entity<Transaction>().HasQueryFilter(t => t.UserId == _userContext.UserIdOrNull);
        }
    }
}

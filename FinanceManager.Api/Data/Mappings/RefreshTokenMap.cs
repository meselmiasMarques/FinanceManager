using FinanceManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManager.Data.Mappings
{
    public class RefreshTokenMap : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_token");

            builder.HasKey(r => r.Id).HasName("PK_RefreshToken");

            builder.Property(r => r.Id)
                .HasColumnName("Id")
                .HasColumnType("uuid")
                .ValueGeneratedNever();

            builder.Property(r => r.UserId)
                .HasColumnName("UserId")
                .HasColumnType("uuid")
                .IsRequired();

            builder.Property(r => r.TokenHash)
                .HasColumnName("TokenHash")
                .HasColumnType("varchar(64)")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(r => r.ExpiresAtUtc)
                .HasColumnName("ExpiresAtUtc")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(r => r.CreatedAtUtc)
                .HasColumnName("CreatedAtUtc")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(r => r.CreatedByIp)
                .HasColumnName("CreatedByIp")
                .HasColumnType("varchar(45)")
                .HasMaxLength(45);

            builder.Property(r => r.RevokedAtUtc)
                .HasColumnName("RevokedAtUtc")
                .HasColumnType("timestamp with time zone");

            builder.Property(r => r.RevokedByIp)
                .HasColumnName("RevokedByIp")
                .HasColumnType("varchar(45)")
                .HasMaxLength(45);

            builder.Property(r => r.ReplacedByTokenHash)
                .HasColumnName("ReplacedByTokenHash")
                .HasColumnType("varchar(64)")
                .HasMaxLength(64);

            builder.Ignore(r => r.IsActive);

            builder.HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .HasConstraintName("FK_RefreshToken_AppUser")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => r.TokenHash)
                .IsUnique()
                .HasDatabaseName("IX_refresh_token_TokenHash");

            builder.HasIndex(r => r.UserId)
                .HasDatabaseName("IX_refresh_token_UserId");
        }
    }
}

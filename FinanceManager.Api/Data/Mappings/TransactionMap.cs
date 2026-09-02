using FinanceManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManager.Data.Mappings
{
    public class TransactionMap : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("transaction");

            builder.HasKey(t => t.Id)
                .HasName("PK_Transaction");

            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id")
                .HasColumnType("integer");

            builder.Property(t => t.Title)
                .HasColumnName("Title")
                .HasColumnType("varchar(100)")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(t => t.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasColumnType("timestamp with time zone");

            builder.Property(t => t.Type)
                .HasColumnName("Type")
                .HasColumnType("integer")
                .IsRequired();

            builder.Property(t => t.UserId)
               .HasColumnName("UserId")
               .HasColumnType("uuid")
               .IsRequired();

            builder.Property(t => t.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .HasConstraintName("FK_Transaction_AppUser")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .HasConstraintName("FK_Transaction_Category")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => new { t.UserId, t.CreatedAt })
                .HasDatabaseName("IX_transaction_UserId_CreatedAt");

            builder.HasIndex(t => t.CategoryId)
                .HasDatabaseName("IX_transaction_CategoryId");
        }
    }
}

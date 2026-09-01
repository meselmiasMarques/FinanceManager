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
                .IsRequired();
            
            builder.Property(t => t.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(t => t.UpdateAt)
                .HasColumnName("UpdateAt")
                .HasColumnType("timestamp");

            builder.Property(t => t.Type)
                .HasColumnName("Type")
                .HasColumnType("integer")
                .IsRequired();

            builder.Property(c => c.UserId)
               .HasColumnName("UserId")
               .HasColumnType("integer");

            builder.Property(t => t.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();


            builder.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .HasConstraintName("FK_Transaction_Category")
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

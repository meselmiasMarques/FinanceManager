using FinanceManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManager.Data.Mappings
{
    public class CategoryMap : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("category");

            builder.HasKey(c => c.Id)
                .HasName("PK_Category");

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id")
                .HasColumnType("integer");

            builder.Property(c => c.Name)
                .HasColumnName("Name")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnName("Description")
                .HasColumnType("varchar(200)")
                .HasMaxLength(200);

            builder.Property(c => c.UserId)
                .HasColumnName("UserId")
                .HasColumnType("integer")
                .IsRequired();

            builder.HasIndex(c => c.UserId)
                .HasDatabaseName("IX_category_UserId");
        }
    }
}

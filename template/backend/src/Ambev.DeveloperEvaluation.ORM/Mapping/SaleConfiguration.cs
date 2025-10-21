using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.SaleNumber).IsRequired();
        builder.Property(s => s.CustomerId).HasColumnType("uuid").IsRequired();
        builder.Property(s => s.BranchId).HasColumnType("uuid").IsRequired();
        builder.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasMany(s => s.Products)
            .WithOne()
            .HasForeignKey(si => si.SaleId);
    }
}

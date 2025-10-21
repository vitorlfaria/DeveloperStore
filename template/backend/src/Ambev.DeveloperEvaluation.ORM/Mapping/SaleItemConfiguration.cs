using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.Property<Guid>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.Property(s => s.SaleId).IsRequired();
        builder.Property(s => s.ProductId).IsRequired();
        builder.Property(s => s.Quantity).IsRequired();
        builder.Property(s => s.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(s => s.DiscountPercentage).IsRequired();
        builder.Property(s => s.Total).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(s => s.IsCanceled).IsRequired();
    }
}

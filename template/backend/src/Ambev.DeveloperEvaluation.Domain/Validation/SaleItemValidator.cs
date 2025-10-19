using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(s => s.ProductId).NotNull().NotEqual(Guid.Empty).WithMessage("Product id is cannot be null");
        RuleFor(s => s.ProductName).NotNull().NotEmpty().WithMessage("Product name cannot be empty or null");
        RuleFor(s => s.Quantity).GreaterThan(0).WithMessage("Sale item quatity must be grater than 0");
        RuleFor(s => s.UnitPrice).GreaterThan(0).WithMessage("Sale item unit price must be grater than 0");
        RuleFor(s => s.DiscountPercentage).LessThanOrEqualTo(20).WithMessage("Sale item discount percentage must be less than or equal to 20");
        RuleFor(s => s.Total).GreaterThan(0).WithMessage("Sale item total must be grater than 0");
    }
}

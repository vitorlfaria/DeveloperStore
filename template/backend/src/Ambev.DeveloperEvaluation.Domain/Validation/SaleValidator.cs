using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(s => s.CustomerId).NotNull().NotEqual(Guid.Empty).WithMessage("Customer id must not be null or empty");
        RuleFor(s => s.BranchId).NotNull().NotEqual(Guid.Empty).WithMessage("Branch id must not be null or empty");
        RuleFor(s => s.Products).NotEmpty().WithMessage("Sale items must not be empty");
        RuleFor(s => s.Products).ForEach(s => s.SetValidator(new SaleItemValidator()));
        RuleFor(s => s.TotalAmount).GreaterThan(0).WithMessage("Total amount must be grater than 0");
        RuleFor(s => s.Status).NotEqual(SaleStatus.Unknown).WithMessage("Sale status cannot be unkown");
    }
}

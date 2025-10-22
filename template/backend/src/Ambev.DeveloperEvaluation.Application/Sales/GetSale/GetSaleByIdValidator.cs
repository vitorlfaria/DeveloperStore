using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleByIdValidator : AbstractValidator<GetSaleByIdQuery>
{
    public GetSaleByIdValidator()
    {
        RuleFor(q => q.Id).NotNull().NotEqual(Guid.Empty);
    }
}

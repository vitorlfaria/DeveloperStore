using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Bogus;

public static class CreateSaleHandlerTestData
{
    private static readonly Faker Faker = new("pt_BR");

    public static CreateSaleCommand GenerateValidCommand()
    {
        return new Faker<CreateSaleCommand>("pt_BR")
            .RuleFor(c => c.CustomerId, _ => Guid.NewGuid())
            .RuleFor(c => c.BranchId, _ => Guid.NewGuid())
            .RuleFor(c => c.Products, _ => SaleTestData.CreateMultipleValidItems())
            .RuleFor(c => c.TotalAmount, f => f.Finance.Amount(1, 1000))
            .Generate();
    }

    public static Sale GenerateValidSaleFromCommand(CreateSaleCommand command)
    {
        var sale = new Sale(
            command.CustomerId,
            command.BranchId
        );
        sale.TotalAmount = command.TotalAmount;

        foreach (var item in command.Products)
            sale.Products.Add(item);

        return sale;
    }
}

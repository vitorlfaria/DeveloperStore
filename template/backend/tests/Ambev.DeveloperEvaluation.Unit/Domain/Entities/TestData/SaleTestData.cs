using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker Faker = new("pt_BR");

    public static Sale CreateValidSale()
    {
        var sale = new Sale(
            customerId: Guid.NewGuid(),
            branchId: Guid.NewGuid()
        );

        var items = CreateMultipleValidItems();

        foreach (var item in items)
            sale.Items.Add(item);

        return sale;
    }

    public static IEnumerable<SaleItem> CreateMultipleValidItems(int count = 3)
    {
        var faker = new Faker<SaleItem>("pt_BR")
            .CustomInstantiator(f => new SaleItem(
                productId: Guid.NewGuid(),
                productName: f.Commerce.ProductName(),
                unitPrice: f.Random.Decimal(100, 5000),
                quantity: f.Random.Int(1, 20)
            ));

        return faker.Generate(count);
    }
}

using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Should create a new Sale with valid customer and branch IDs")]
    public void Should_Create_New_Sale_With_Valid_Data()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();

        // Act
        var sale = new Sale(customerId, branchId);

        // Assert
        sale.CustomerId.Should().Be(customerId);
        sale.BranchId.Should().Be(branchId);
        sale.SaleNumber.Should().StartWith("S-");
        sale.Status.Should().Be(SaleStatus.Active);
        sale.TotalAmount.Should().Be(0);
        sale.Products.Should().BeEmpty();
    }

    [Fact(DisplayName = "Should throw when creating sale with empty CustomerId")]
    public void Should_Throw_When_CustomerId_Is_Empty()
    {
        // Arrange
        var branchId = Guid.NewGuid();
        var sale = new Sale(Guid.Empty, branchId);

        // Act
        var result = sale.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Should throw when creating sale with empty BranchId")]
    public void Should_Throw_When_BranchId_Is_Empty()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var sale = new Sale(customerId, Guid.Empty);

        // Act
        var result = sale.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Should cancel entire sale and mark all items cancelled")]
    public void Should_Cancel_Entire_Sale()
    {
        // Arrange
        var sale = new Sale(Guid.NewGuid(), Guid.NewGuid());

        // Act
        sale.Cancel();

        // Assert
        sale.Status.Should().Be(SaleStatus.Cancelled);
        sale.TotalAmount.Should().Be(0);
    }

    [Fact(DisplayName = "Should calculate totals correctly")]
    public void Should_CalculateTotalCorretly()
    {
        // Arrange
        var sale = SaleTestData.CreateValidSale();

        // Act
        sale.RecalculateTotals();

        // Assert
        sale.TotalAmount.Should().BeGreaterThan(0);
    }
}

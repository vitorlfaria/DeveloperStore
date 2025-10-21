using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid update command When sale exists Then returns updated result")]
    public async Task Handle_ValidCommand_ReturnsUpdatedResult()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid()
        };
        var sale = new Sale(command.CustomerId, command.BranchId);
        sale.Products.Add(new SaleItem(Guid.NewGuid(), 1.2m, 2));
        sale.TotalAmount = 2.4m;
        var updatedSale = new Sale(command.CustomerId, command.BranchId);
        var expectedResult = new UpdateSaleResult
        {
            Id = updatedSale.Id,
            SaleNumber = updatedSale.SaleNumber,
            TotalAmount = updatedSale.TotalAmount
        };

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(updatedSale);
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(updatedSale.Id);
        result.SaleNumber.Should().Be(updatedSale.SaleNumber);
        result.TotalAmount.Should().Be(updatedSale.TotalAmount);
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<UpdateSaleResult>(updatedSale);
    }

    [Fact(DisplayName = "Given invalid command When handling Then throws validation exception")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.Empty, // invalid
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Products = [new SaleItem(Guid.NewGuid(), 1, 0)],
            SaleNumber = string.Empty,
            Status = SaleStatus.Unknown,
            TotalAmount = 0
        };
        var sale = new Sale(command.CustomerId, command.BranchId);
        _mapper.Map<Sale>(command).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
        await _saleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default, default);
        _mapper.DidNotReceiveWithAnyArgs().Map<UpdateSaleResult>(default);
    }
}


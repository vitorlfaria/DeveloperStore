using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _handler = new DeleteSaleHandler(_saleRepository);
    }

    [Fact(DisplayName = "Given valid delete command When handling Then returns success result")]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Given
        var command = new DeleteSaleCommand { Id = Guid.NewGuid() };
        var sale = new Sale(Guid.NewGuid(), Guid.NewGuid());
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        await _saleRepository.Received(1).DeleteAsync(command.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid delete command When deletion fails Then returns failure result")]
    public async Task Handle_ValidCommand_DeletionFails_ReturnsFalse()
    {
        // Given
        var command = new DeleteSaleCommand { Id = Guid.NewGuid() };
        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        await _saleRepository.Received(1).DeleteAsync(command.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid delete command When handling Then throws validation exception")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Given
        var command = new DeleteSaleCommand { Id = Guid.Empty };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
        await _saleRepository.DidNotReceiveWithAnyArgs().DeleteAsync(default, default);
    }
}

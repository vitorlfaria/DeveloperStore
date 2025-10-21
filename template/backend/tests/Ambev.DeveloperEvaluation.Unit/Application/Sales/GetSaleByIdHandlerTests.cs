using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleByIdHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleByIdHandler _handler;

    public GetSaleByIdHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleByIdHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid query When sale exists Then returns sale result")]
    public async Task Handle_ValidQuery_SaleExists_ReturnsResult()
    {
        // Given
        var profile = new GetSaleByIdProfile();
        var query = new GetSaleByIdQuery { Id = Guid.NewGuid() };
        var sale = new Sale(Guid.NewGuid(), Guid.NewGuid());
        var expectedResult = new GetSaleByIdResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            TotalAmount = sale.TotalAmount
        };

        _saleRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleByIdResult>(sale).Returns(expectedResult);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        result.SaleNumber.Should().Be(sale.SaleNumber);
        result.TotalAmount.Should().Be(sale.TotalAmount);
        await _saleRepository.Received(1).GetByIdAsync(query.Id, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<GetSaleByIdResult>(sale);
    }

    [Fact(DisplayName = "Given valid query When sale does not exist Then returns null")]
    public async Task Handle_ValidQuery_SaleDoesNotExist_ReturnsNull()
    {
        // Given
        var query = new GetSaleByIdQuery { Id = Guid.NewGuid() };
        _saleRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns((Sale)null);
        _mapper.Map<GetSaleByIdResult>(null).Returns((GetSaleByIdResult)null);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        result.Should().BeNull();
        await _saleRepository.Received(1).GetByIdAsync(query.Id, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<GetSaleByIdResult>(null);
    }

    [Fact(DisplayName = "Given invalid query When handling Then throws validation exception")]
    public async Task Handle_InvalidQuery_ThrowsValidationException()
    {
        // Given
        var query = new GetSaleByIdQuery { Id = Guid.Empty }; // invalid

        // When
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
        await _saleRepository.DidNotReceiveWithAnyArgs().GetByIdAsync(default, default);
        _mapper.DidNotReceiveWithAnyArgs().Map<GetSaleByIdResult>(default);
    }
}

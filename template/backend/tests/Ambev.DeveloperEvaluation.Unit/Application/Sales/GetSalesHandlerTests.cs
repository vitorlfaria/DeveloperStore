using FluentAssertions;
using NSubstitute;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.GetSalesList;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSalesHandlerTests
{
    private readonly DbContextOptions<DefaultContext> _options;
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSalesHandler _handler;

    public GetSalesHandlerTests()
    {
        _options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"UserRepositoryTests_{Guid.NewGuid()}")
            .Options;
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given query without filters When handler called Then returns paged sales")]
    public async Task Handle_NoFilters_ReturnsPagedSales()
    {
        // Given
        var salesList = Enumerable.Range(1, 5)
            .Select(i => new Sale(Guid.NewGuid(), Guid.NewGuid()))
            .ToList();
        var repository = new SaleRepository(new DefaultContext(_options));

        foreach (var sale in salesList)
        {
            await repository.CreateAsync(sale);
        }

        var query = new GetSalesQuery
        {
            Page = 1,
            Size = 10
        };

        var handler = new GetSalesHandler(repository, _mapper);

        // When
        var result = await handler.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Count().Should().Be(salesList.Count);
    }

    [Fact(DisplayName = "Given query with filters When handler called Then applies filters")]
    public async Task Handle_WithFilters_ReturnsFilteredSales()
    {
        // Given
        var customerId = Guid.NewGuid();
        var salesList = new List<Sale>
        {
            new(customerId, Guid.NewGuid()),
            new(Guid.NewGuid(), Guid.NewGuid())
        };
        var repository = new SaleRepository(new DefaultContext(_options));

        foreach (var sale in salesList)
        {
            await repository.CreateAsync(sale);
        }

        var query = new GetSalesQuery
        {
            Page = 1,
            Size = 10,
            Filters = new Dictionary<string, string>
            {
                { "CustomerId", customerId.ToString() }
            }
        };
        var handler = new GetSalesHandler(repository, _mapper);

        // When
        var result = await handler.Handle(query, CancellationToken.None);

        // Then
        result.Should().HaveCount(1);
        result.First().CustomerId.Should().Be(customerId);
    }
}

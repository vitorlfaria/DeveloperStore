using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.ORM.Repositories;

public class SaleRepositoryTests
{
    private readonly DbContextOptions<DefaultContext> _options;

    public SaleRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"UserRepositoryTests_{Guid.NewGuid()}")
            .Options;
    }

    [Fact(DisplayName = "Should intantiate repository successfully")]
    public async Task Should_Instantiate_Successfully()
    {
        // Arrange
        using var context = new DefaultContext(_options);

        // Act
        var repository = new SaleRepository(context);

        // Assert
        repository.Should().NotBeNull();
    }
}

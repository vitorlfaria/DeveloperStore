using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using YourNamespace.Tests.Repositories.TestData;

namespace Ambev.DeveloperEvaluation.Integration.ORM.Repositories;

public class UserRepositoryTests
{
    private readonly DbContextOptions<DefaultContext> _options;

    public UserRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"UserRepositoryTests_{Guid.NewGuid()}")
            .Options;
    }

    [Fact(DisplayName = "Should create a user successfully")]
    public async Task CreateAsync_ShouldCreateUserSuccessfully()
    {
        using var context = new DefaultContext(_options);
        var repository = new UserRepository(context);
        var user = UserRepositoryTestData.GenerateValidUser();

        var result = await repository.CreateAsync(user);

        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        context.Users.Should().ContainSingle(u => u.Email == user.Email);
    }

    [Fact(DisplayName = "Should retrieve user by ID successfully")]
    public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
    {
        using var context = new DefaultContext(_options);
        var repository = new UserRepository(context);
        var user = UserRepositoryTestData.GenerateValidUser();

        await repository.CreateAsync(user);

        var result = await repository.GetByIdAsync(user.Id);

        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
    }

    [Fact(DisplayName = "Should return null when user ID does not exist")]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserNotFound()
    {
        using var context = new DefaultContext(_options);
        var repository = new UserRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact(DisplayName = "Should logically delete user and mark as deleted")]
    public async Task DeleteAsync_ShouldMarkUserAsDeleted()
    {
        using var context = new DefaultContext(_options);
        var repository = new UserRepository(context);
        var user = UserRepositoryTestData.GenerateValidUser();

        await repository.CreateAsync(user);

        var deleted = await repository.DeleteAsync(user.Id);

        deleted.Should().NotBeNull();

        var retrieved = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        retrieved.Should().NotBeNull();
        retrieved!.IsDeleted.Should().BeTrue();
    }

    [Fact(DisplayName = "Should return false on delete when user does not exists")]
    public async Task DeleteAsync_ShouldReturnFalse()
    {
        using var context = new DefaultContext(_options);
        var repository = new UserRepository(context);

        var deleted = await repository.DeleteAsync(Guid.Empty);

        deleted.Should().BeNull();
    }

    [Fact(DisplayName = "Should return user by email when email exists")]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        using var context = new DefaultContext(_options);
        var repository = new UserRepository(context);
        var user = UserRepositoryTestData.GenerateValidUser();

        await repository.CreateAsync(user);

        var result = await repository.GetByEmailAsync(user.Email);

        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
    }

    [Fact(DisplayName = "Should return null when email does not exist")]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailNotFound()
    {
        using var context = new DefaultContext(_options);
        var repository = new UserRepository(context);

        var result = await repository.GetByEmailAsync("nonexistent@email.com");

        result.Should().BeNull();
    }

    [Fact(DisplayName = "Should return user by expression when exists")]
    public async Task GetOneByExpression_ShouldReturnUser()
    {
        using var context = new DefaultContext(_options);
        var repository = new UserRepository(context);
        var user = UserRepositoryTestData.GenerateValidUser();

        await repository.CreateAsync(user);

        var result = await repository.GetOneByExpression(u => u.Email == user.Email);

        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
    }

    [Fact(DisplayName = "Should return null by expression when not exists")]
    public async Task GetOneByExpression_ShouldReturnNull()
    {
        using var context = new DefaultContext(_options);
        var repository = new UserRepository(context);

        var result = await repository.GetOneByExpression(u => u.Email == "nonexisting@email.com");

        result.Should().BeNull();
    }
}

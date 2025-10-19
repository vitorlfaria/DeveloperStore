using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

/// <summary>
/// Contains unit tests for the <see cref="AuthenticateUserHandler"/> class.
/// </summary>
public class AuthenticateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly AuthenticateUserHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticateUserHandlerTests"/> class.
    /// Sets up fake dependencies.
    /// </summary>
    public AuthenticateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        _handler = new AuthenticateUserHandler(_userRepository, _passwordHasher, _jwtTokenGenerator);
    }

    /// <summary>
    /// Tests that a valid authentication request returns a valid token and user info.
    /// </summary>
    [Fact(DisplayName = "Given valid credentials When authenticating Then returns valid token and user info")]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        // Given
        var command = AuthenticateUserHandlerTestData.GenerateValidCommand();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "john_doe",
            Email = command.Email,
            Password = "hashedPass",
            Role = UserRole.Admin,
            Status = UserStatus.Active
        };

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password).Returns(true);
        _jwtTokenGenerator.GenerateToken(user).Returns("fake-jwt-token");

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Token.Should().Be("fake-jwt-token");
        result.Email.Should().Be(user.Email);
        result.Name.Should().Be(user.Username);
        result.Role.Should().Be(user.Role.ToString());

        await _userRepository.Received(1).GetByEmailAsync(command.Email, Arg.Any<CancellationToken>());
        _passwordHasher.Received(1).VerifyPassword(command.Password, user.Password);
        _jwtTokenGenerator.Received(1).GenerateToken(user);
    }

    /// <summary>
    /// Tests that an invalid password throws an UnauthorizedAccessException.
    /// </summary>
    [Fact(DisplayName = "Given invalid password When authenticating Then throws UnauthorizedAccessException")]
    public async Task Handle_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Given
        var command = AuthenticateUserHandlerTestData.GenerateValidCommand();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "john_doe",
            Email = command.Email,
            Password = "hashedPass",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password).Returns(false);

        // When
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");

        _jwtTokenGenerator.DidNotReceive().GenerateToken(Arg.Any<User>());
    }

    /// <summary>
    /// Tests that when user does not exist, throws UnauthorizedAccessException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent user email When authenticating Then throws UnauthorizedAccessException")]
    public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        // Given
        var command = AuthenticateUserHandlerTestData.GenerateValidCommand();
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns((User?)null);

        // When
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");

        _jwtTokenGenerator.DidNotReceive().GenerateToken(Arg.Any<User>());
    }

    /// <summary>
    /// Tests that when the user is inactive, throws UnauthorizedAccessException.
    /// </summary>
    [Fact(DisplayName = "Given inactive user When authenticating Then throws UnauthorizedAccessException")]
    public async Task Handle_InactiveUser_ThrowsUnauthorizedAccessException()
    {
        // Given
        var command = AuthenticateUserHandlerTestData.GenerateValidCommand();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "john_doe",
            Email = command.Email,
            Password = "hashedPass",
            Role = UserRole.Customer,
            Status = UserStatus.Suspended
        };

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password).Returns(true);

        // When
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User is not active");

        _jwtTokenGenerator.DidNotReceive().GenerateToken(Arg.Any<User>());
    }
}

using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

/// <summary>
/// Contains unit tests for the <see cref="GetUserHandler"/> class.
/// </summary>
public class GetUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly GetUserHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserHandlerTests"/> class.
    /// Sets up the test dependencies and fakes.
    /// </summary>
    public GetUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetUserHandler(_userRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid user request returns the correct user data.
    /// </summary>
    [Fact(DisplayName = "Given valid user ID When handling Then returns mapped user result")]
    public async Task Handle_ValidRequest_ReturnsMappedUserResult()
    {
        // Given
        var profile = new GetUserProfile();
        var command = GetUserHandlerTestData.GenerateValidCommand();
        var user = new User
        {
            Id = command.Id,
            Username = "john_doe",
            Email = "john@example.com",
            Password = "hashedPass",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        var expectedResult = new GetUserResult
        {
            Id = user.Id,
            Name = user.Username,
            Email = user.Email
        };

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(user);
        _mapper.Map<GetUserResult>(user).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Username);
        result.Email.Should().Be(user.Email);
        await _userRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<GetUserResult>(user);
    }

    /// <summary>
    /// Tests that when user does not exist, throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent user ID When handling Then throws KeyNotFoundException")]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = GetUserHandlerTestData.GenerateValidCommand();
        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((User?)null);

        // When
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"User with ID {command.Id} not found");

        await _userRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        _mapper.DidNotReceive().Map<GetUserResult>(Arg.Any<User>());
    }

    /// <summary>
    /// Tests that invalid request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid user ID When handling Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = GetUserHandlerTestData.GenerateInvalidCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
        await _userRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}

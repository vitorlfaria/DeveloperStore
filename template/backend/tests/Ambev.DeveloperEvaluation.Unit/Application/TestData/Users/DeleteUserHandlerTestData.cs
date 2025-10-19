using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Bogus;

/// <summary>
/// Provides methods for generating test data for the <see cref="DeleteUserHandler"/> tests
/// using the Bogus library. This ensures consistency and realism across test scenarios.
/// </summary>
public static class DeleteUserHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid DeleteUserCommand instances.
    /// The generated command will contain:
    /// - A non-empty valid Guid for Id
    /// </summary>
    private static readonly Faker<DeleteUserCommand> deleteUserHandlerFaker = new Faker<DeleteUserCommand>()
        .RuleFor(c => c.Id, f => f.Random.Guid());

    /// <summary>
    /// Generates a valid DeleteUserCommand with a non-empty Guid.
    /// Useful for positive test cases where validation should pass.
    /// </summary>
    /// <returns>A valid <see cref="DeleteUserCommand"/> instance.</returns>
    public static DeleteUserCommand GenerateValidCommand()
    {
        return deleteUserHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates an invalid DeleteUserCommand with an empty Guid.
    /// Useful for negative test cases where validation should fail.
    /// </summary>
    /// <returns>An invalid <see cref="DeleteUserCommand"/> instance.</returns>
    public static DeleteUserCommand GenerateInvalidCommand()
    {
        return new DeleteUserCommand { Id = Guid.Empty };
    }
}

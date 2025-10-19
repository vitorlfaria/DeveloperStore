using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Bogus;

/// <summary>
/// Provides methods for generating test data for the <see cref="GetUserHandler"/> tests
/// using the Bogus library. Ensures consistent and realistic data generation.
/// </summary>
public static class GetUserHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid GetUserCommand instances.
    /// The generated command will contain:
    /// - A non-empty valid Guid for Id
    /// </summary>
    private static readonly Faker<GetUserCommand> getUserHandlerFaker = new Faker<GetUserCommand>()
        .RuleFor(c => c.Id, f => f.Random.Guid());

    /// <summary>
    /// Generates a valid GetUserCommand with a non-empty Guid.
    /// Useful for positive test cases where validation should pass.
    /// </summary>
    /// <returns>A valid <see cref="GetUserCommand"/> instance.</returns>
    public static GetUserCommand GenerateValidCommand()
    {
        return getUserHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates an invalid GetUserCommand with an empty Guid.
    /// Useful for negative test cases where validation should fail.
    /// </summary>
    /// <returns>An invalid <see cref="GetUserCommand"/> instance.</returns>
    public static GetUserCommand GenerateInvalidCommand()
    {
        return new GetUserCommand { Id = Guid.Empty };
    }
}

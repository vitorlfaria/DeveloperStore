using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Bogus;

/// <summary>
/// Provides methods for generating test data for the <see cref="AuthenticateUserHandler"/> tests
/// using the Bogus library. Ensures consistent and realistic authentication scenarios.
/// </summary>
public static class AuthenticateUserHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid AuthenticateUserCommand instances.
    /// The generated command will contain:
    /// - A valid email in correct format
    /// - A valid password that meets basic complexity requirements
    /// </summary>
    private static readonly Faker<AuthenticateUserCommand> authenticateUserHandlerFaker = new Faker<AuthenticateUserCommand>()
        .RuleFor(c => c.Email, f => f.Internet.Email())
        .RuleFor(c => c.Password, f => $"Test@{f.Random.Number(100, 999)}");

    /// <summary>
    /// Generates a valid AuthenticateUserCommand with realistic data.
    /// Useful for positive test cases where validation should pass.
    /// </summary>
    /// <returns>A valid <see cref="AuthenticateUserCommand"/> instance.</returns>
    public static AuthenticateUserCommand GenerateValidCommand()
    {
        return authenticateUserHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates an invalid AuthenticateUserCommand with missing or invalid fields.
    /// Useful for negative test cases where validation should fail.
    /// </summary>
    /// <returns>An invalid <see cref="AuthenticateUserCommand"/> instance.</returns>
    public static AuthenticateUserCommand GenerateInvalidCommand()
    {
        return new AuthenticateUserCommand
        {
            Email = string.Empty,
            Password = string.Empty
        };
    }
}

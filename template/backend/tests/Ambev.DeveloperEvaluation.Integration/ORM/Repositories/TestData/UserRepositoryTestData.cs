using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace YourNamespace.Tests.Repositories.TestData
{
    /// <summary>
    /// Provides test data for UserRepository tests.
    /// </summary>
    public static class UserRepositoryTestData
    {
        private static readonly Faker<User> userFaker = new Faker<User>("pt_BR")
            .RuleFor(u => u.Id, f => Guid.NewGuid())
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => $"Test@{f.Random.Number(100, 999)}")
            .RuleFor(u => u.Phone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}")
            .RuleFor(u => u.Role, f => f.PickRandom<UserRole>())
            .RuleFor(u => u.Status, f => f.PickRandom<UserStatus>());

        /// <summary>
        /// Generates a valid user entity with randomized but valid data.
        /// </summary>
        public static User GenerateValidUser() => userFaker.Generate();

        /// <summary>
        /// Generates multiple valid users.
        /// </summary>
        public static IEnumerable<User> GenerateUsers(int count = 5) => userFaker.Generate(count);
    }
}


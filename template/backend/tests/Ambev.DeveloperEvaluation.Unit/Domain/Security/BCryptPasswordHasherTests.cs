using Xunit;
using FluentAssertions;
using Ambev.DeveloperEvaluation.Common.Security;

namespace Ambev.DeveloperEvaluation.Tests.Security
{
    public class BCryptPasswordHasherTests
    {
        private readonly BCryptPasswordHasher _hasher;

        public BCryptPasswordHasherTests()
        {
            _hasher = new BCryptPasswordHasher();
        }

        [Fact(DisplayName = "Should generate a valid hash different from the original password")]
        public void HashPassword_ShouldReturnDifferentValue_ThanOriginalPassword()
        {
            // Arrange
            var password = "StrongPassword@123";

            // Act
            var hash = _hasher.HashPassword(password);

            // Assert
            hash.Should().NotBeNullOrWhiteSpace();
            hash.Should().NotBe(password);
        }

        [Fact(DisplayName = "Should verify password successfully when hash is valid")]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatchesHash()
        {
            // Arrange
            var password = "MySecurePassword!";
            var hash = _hasher.HashPassword(password);

            // Act
            var result = _hasher.VerifyPassword(password, hash);

            // Assert
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "Should fail verification when password does not match hash")]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatchHash()
        {
            // Arrange
            var correctPassword = "CorrectPassword123!";
            var wrongPassword = "WrongPassword!";
            var hash = _hasher.HashPassword(correctPassword);

            // Act
            var result = _hasher.VerifyPassword(wrongPassword, hash);

            // Assert
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "Should generate different hashes for the same password due to salting")]
        public void HashPassword_ShouldGenerateDifferentHashes_ForSamePassword()
        {
            // Arrange
            var password = "RepeatablePassword123!";

            // Act
            var hash1 = _hasher.HashPassword(password);
            var hash2 = _hasher.HashPassword(password);

            // Assert
            hash1.Should().NotBe(hash2);
            _hasher.VerifyPassword(password, hash1).Should().BeTrue();
            _hasher.VerifyPassword(password, hash2).Should().BeTrue();
        }

        [Fact(DisplayName = "Should throw ArgumentNullException when trying to hash a null password")]
        public void HashPassword_ShouldThrow_WhenPasswordIsNull()
        {
            // Act
            Action act = () => _hasher.HashPassword(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact(DisplayName = "Should return throw an ArgumentNullException when verifying a null password or hash")]
        public void VerifyPassword_ShouldReturnFalse_WhenArgumentsAreNull()
        {
            // Act
            var result1 = Assert.Throws<ArgumentNullException>(() => _hasher.VerifyPassword(null!, "somehash"));
            var result2 = Assert.Throws<ArgumentNullException>(() => _hasher.VerifyPassword("password", null!));

            // Assert
            result1.Should().BeOfType<ArgumentNullException>();
            result2.Should().BeOfType<ArgumentNullException>();
        }
    }
}

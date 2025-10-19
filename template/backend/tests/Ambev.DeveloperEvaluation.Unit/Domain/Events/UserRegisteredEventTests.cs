using System;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Events;

public class UserRegisteredEventTests
{
    [Fact(DisplayName = "UserRegisteredEvent should be instantiated correctly")]
    public void Given_UserRegisteredEvent_When_Instantiated_Should_BeCorrect()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();

        // Act
        UserRegisteredEvent userRegisteredEvent = new(user);

        // Assert
        Assert.Equal(user, userRegisteredEvent.User);
    }
}

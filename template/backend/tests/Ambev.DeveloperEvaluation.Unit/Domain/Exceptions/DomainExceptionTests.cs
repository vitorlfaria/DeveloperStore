using System;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact()]
    public void Given_DomainException_When_Instantiated_Should_BeCorrect()
    {
        // Arrange
        var message = "Exception message";

        // Act
        var exception = new DomainException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact()]
    public void Given_DomainException_When_Instantiated_Should_BeHaveCorrectInnerException()
    {
        // Arrange
        var message = "Exception message";
        var inner = new Exception("test");

        // Act
        var exception = new DomainException(message, inner);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(inner, exception.InnerException);
    }
}

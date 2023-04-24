using Microsoft.AspNetCore.Http;
using MPay.Infrastructure.ErrorHandling;
using MPay.Infrastructure.Tests.ErrorHandling.Exceptions;
using MockException = MPay.Infrastructure.Tests.ErrorHandling.Exceptions.MockException;

namespace MPay.Infrastructure.Tests.ErrorHandling;

public class ExceptionMapperTests
{
    [Fact]
    public void Map_MapMPayExceptionToErrorDetails()
    {
        // Arrange
        var mockException = AutoFaker.Generate<MockException>();
        var exceptionMapper = new ExceptionMapper();
        
        // Act
        var errorDetails = exceptionMapper.Map(mockException);
        
        // Assert
        errorDetails.Should().NotBeNull();
        errorDetails.Title.Should().Be(mockException.Message);
        errorDetails.ErrorCode.Should().Be("MOCK");
        errorDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
        errorDetails.Data.Should().NotBeNull();
    }

    [Fact]
    public void Map_MapMPayExceptionWithDataToErrorDetails()
    {
        // Arrange
        var mockWithDataException = AutoFaker.Generate<MockWithDataException>();
        var exceptionMapper = new ExceptionMapper();
        
        // Act
        var errorDetails = exceptionMapper.Map(mockWithDataException);
        
        // Assert
        errorDetails.Should().NotBeNull();
        errorDetails.Title.Should().Be(mockWithDataException.Message);
        errorDetails.ErrorCode.Should().Be("MOCK_WITH_DATA");
        errorDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
        errorDetails.Data.Should().NotBeNull();
        errorDetails.Data.Should().ContainKey("id");
        errorDetails.Data.Should().ContainKey("value");
        errorDetails.Data["id"].Should().Be(mockWithDataException.Id);
        errorDetails.Data["value"].Should().Be(mockWithDataException.Value);
    }

    [Fact]
    public void Map_MapExceptionToErrorDetails()
    {
        // Arrange
        var exception = AutoFaker.Generate<Exception>();
        var exceptionMapper = new ExceptionMapper();
        
        // Act
        var errorDetails = exceptionMapper.Map(exception);
        
        // Assert
        errorDetails.Should().NotBeNull();
        errorDetails.Title.Should().Be("Unexpected error occurred.");
        errorDetails.ErrorCode.Should().Be("UNEXPECTED_ERROR");
        errorDetails.Status.Should().Be(StatusCodes.Status500InternalServerError);
        errorDetails.Data.Should().BeNull();
    }

    [Fact]
    public void Map_MapNotFoundExceptionToErrorDetails()
    {
        // Arrange
        var exception = AutoFaker.Generate<MockNotFoundException>();
        var exceptionMapper = new ExceptionMapper();
        
        // Act
        var errorDetails = exceptionMapper.Map(exception);
        
        // Assert
        errorDetails.Should().NotBeNull();
        errorDetails.Title.Should().Be(exception.Message);
        errorDetails.ErrorCode.Should().Be("MOCK_NOT_FOUND");
        errorDetails.Status.Should().Be(StatusCodes.Status404NotFound);
        errorDetails.Data.Should().NotBeNull();
    }
}
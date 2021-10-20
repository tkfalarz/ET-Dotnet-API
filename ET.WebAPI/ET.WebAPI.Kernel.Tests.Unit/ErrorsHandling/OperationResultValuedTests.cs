using ET.WebAPI.Kernel.ErrorsHandling;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace ET.WebAPI.Kernel.Tests.Unit.ErrorsHandling
{
    [TestFixture]
    public class OperationResultValuedTests
    {
        [Test]
        public void WhenFailureThenOperationResultShouldHaveErrorMessage()
        {
            const string errorMessage = "ErrorMessage"; 
            
            var result = OperationResult<string>.Failure(errorMessage, ErrorType.BusinessLogic);

            result.ErrorMessage.Should().NotBeNull().And.Be(errorMessage);
        }

        [TestCase(ErrorType.BusinessLogic)]
        [TestCase(ErrorType.Api)]
        [TestCase(ErrorType.None)]
        public void WhenFailureThenOperationResultShouldHaveErrorType(ErrorType errorType)
        {
            var result = OperationResult<string>.Failure("ErrorMessage", errorType);

            result.ErrorType.Should().Be(errorType);
        }

        [Test]
        public void WhenFailureThenOperationResultShouldHaveNegativeIsProceededFlag()
        {
            var result = OperationResult<string>.Failure(string.Empty, It.IsAny<ErrorType>());

            result.IsProceeded.Should().BeFalse();
        }

        [Test]
        public void WhenFailureErrorMessageIsNullThenInvalidOperationExceptionIsThrown()
        {
            Action action = () => OperationResult<string>.Failure(default, ErrorType.None);

            action.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Error message cannot be null");
        }
        
        [Test]
        public void WhenProceededThenOperationResultShouldHaveNoneErrorType()
        {
            var result = OperationResult<string>.Proceeded("Any string");

            result.ErrorType.Should().Be(ErrorType.None);
        }
        
        [Test]
        public void WhenProceededThenOperationResultValueShouldHaveValue()
        {
            var expectedData = "any data";
            var result = OperationResult<string>.Proceeded(expectedData);

            result.Value.Should().Be(expectedData);
        }

        [Test]
        public void WhenProceededThenOperationResultShouldHaveEmptyErrorMessage()
        {
            var result = OperationResult<string>.Proceeded("any data");

            result.ErrorMessage.Should().BeEmpty();
        }

        [Test]
        public void WhenProceededThenOperationResultShouldHavePositiveIsProceededFlag()
        {
            var result = OperationResult<string>.Proceeded("any data");

            result.IsProceeded.Should().BeTrue();
        }
    }
}
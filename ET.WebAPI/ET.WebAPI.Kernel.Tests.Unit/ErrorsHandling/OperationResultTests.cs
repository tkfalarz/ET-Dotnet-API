using ET.WebAPI.Kernel.ErrorsHandling;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;

namespace ET.WebAPI.Kernel.Tests.Unit.ErrorsHandling
{
    public class OperationResultTests
    {
        [Test]
        public void WhenFailureThenOperationResultShouldHaveErrorMessage()
        {
            const string errorMessage = "ErrorMessage"; 
            
            var result = OperationResult.Failure(errorMessage, ErrorType.BusinessLogic);

            result.ErrorMessage.Should().NotBeNull().And.Be(errorMessage);
        }

        [TestCase(ErrorType.BusinessLogic)]
        [TestCase(ErrorType.Api)]
        [TestCase(ErrorType.None)]
        public void WhenFailureThenOperationResultShouldHaveErrorType(ErrorType errorType)
        {
            var result = OperationResult.Failure("ErrorMessage", errorType);

            result.ErrorType.Should().Be(errorType);
        }

        [Test]
        public void WhenFailureThenOperationResultShouldHaveNegativeIsProceededFlag()
        {
            var result = OperationResult.Failure(string.Empty, It.IsAny<ErrorType>());

            result.IsProceeded.Should().BeFalse();
        }

        [Test]
        public void WhenFailureErrorMessageIsNullThenInvalidOperationExceptionIsThrown()
        {
            Action action = () => OperationResult.Failure(default, ErrorType.None);

            action.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Error message cannot be null");
        }
        
        [Test]
        public void WhenProceededThenOperationResultShouldHaveNoneErrorType()
        {
            var result = OperationResult.Proceeded();

            result.ErrorType.Should().Be(ErrorType.None);
        }

        [Test]
        public void WhenProceededThenOperationResultShouldHaveEmptyErrorMessage()
        {
            var result = OperationResult.Proceeded();

            result.ErrorMessage.Should().BeEmpty();
        }

        [Test]
        public void WhenProceededThenOperationResultShouldHavePositiveIsProceededFlag()
        {
            var result = OperationResult.Proceeded();

            result.IsProceeded.Should().BeTrue();
        }
    }
}
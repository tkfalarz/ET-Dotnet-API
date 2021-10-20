using ET.WebAPI.Kernel.ErrorsHandling;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace ET.WebAPI.Kernel.Tests.Unit.ErrorsHandling
{
    public static class OperationResultTestCases
    {
        public static IEnumerable<TestCaseData> EqualsTestCases()
            => new[]
            {
                new TestCaseData(
                    OperationResult.Failure("errorMessage", ErrorType.Api),
                    OperationResult.Failure("errorMessage", ErrorType.Api),
                    true
                ),
                new TestCaseData(
                    OperationResult.Failure("erroRMesSage", ErrorType.Api),
                    OperationResult.Failure("errorMessage", ErrorType.Api),
                    false
                ),
                new TestCaseData(
                    OperationResult.Failure("errorMessage", ErrorType.Api),
                    OperationResult.Failure("errorMessage", ErrorType.Entity),
                    false
                ),
                new TestCaseData(
                    OperationResult.Failure("error", ErrorType.Api),
                    OperationResult.Failure("errorMessage", ErrorType.Api),
                    false
                ),
                new TestCaseData(
                    OperationResult.Proceeded(),
                    OperationResult.Failure("errorMessage", ErrorType.Api),
                    false
                ),
                new TestCaseData(
                    OperationResult.Proceeded(),
                    null,
                    false
                ),
                new TestCaseData(
                    OperationResult.Proceeded(),
                    OperationResult.Proceeded(),
                    true
                )
            };
    }
}
using System;

namespace ET.WebAPI.Kernel.ErrorsHandling
{
    public class OperationResult
    {
        public string ErrorMessage { get; }
        public ErrorType ErrorType { get; }
        public bool IsProceeded { get; }

        private OperationResult(bool isProceeded, string errorMessage, ErrorType errorType)
        {
            if (errorMessage == default) throw new InvalidOperationException("Error message cannot be null");

            IsProceeded = isProceeded;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

        public static OperationResult Proceeded() => new(true, string.Empty, ErrorType.None);

        public static OperationResult Failure(string errorMessage, ErrorType errorType) => new(false, errorMessage, errorType);
    }
}
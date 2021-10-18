using System;

namespace ET.WebAPI.Kernel.ErrorsHandling
{
    public class OperationResult<T>
    {
        public string ErrorMessage { get; }
        public ErrorType ErrorType { get; }
        public bool IsProceeded { get; }
        public bool IsFailure => !IsProceeded;
        public T Value { get; }

        private OperationResult(bool isProceeded, string errorMessage, ErrorType errorType, T value)
        {
            if (errorMessage == default) throw new InvalidOperationException("Error message cannot be null");

            IsProceeded = isProceeded;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
            Value = value;
        }

        public static OperationResult<T> Proceeded(T value) => new(true, string.Empty, ErrorsHandling.ErrorType.None, value);
        public static OperationResult<T> Failure(string errorMessage, ErrorType errorType) => new(false, errorMessage, errorType, (T)default);
    }
}
using System;

namespace ET.WebAPI.Kernel
{
    public class OperationResult
    {
        public string ErrorMessage { get; }
        public bool IsProceeded { get; }

        private OperationResult(bool isProceeded, string errorMessage)
        {
            IsProceeded = isProceeded;
            ErrorMessage = errorMessage;
        }

        public static OperationResult Proceeded() => new(true, default);

        public static OperationResult Failure(string errorMessage) => new(false, errorMessage);
    }
}
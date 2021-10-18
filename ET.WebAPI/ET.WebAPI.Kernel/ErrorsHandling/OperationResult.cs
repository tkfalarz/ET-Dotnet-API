using System;

namespace ET.WebAPI.Kernel.ErrorsHandling
{
    public class OperationResult
    {
        public string ErrorMessage { get; }
        public ErrorType ErrorType { get; }
        public bool IsProceeded { get; }
        public bool IsFailure => !IsProceeded;
        
        private OperationResult(bool isProceeded, string errorMessage, ErrorType errorType)
        {
            if (errorMessage == default) throw new InvalidOperationException("Error message cannot be null");

            IsProceeded = isProceeded;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

        public static OperationResult Proceeded() => new(true, string.Empty, ErrorsHandling.ErrorType.None);
        public static OperationResult Failure(string errorMessage, ErrorType errorType) => new(false, errorMessage, errorType);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OperationResult)obj);
        }

        public override int GetHashCode() => HashCode.Combine(ErrorMessage, (int)ErrorType, IsProceeded);

        public bool Equals(OperationResult other)
            => ErrorMessage == other.ErrorMessage
               && ErrorType == other.ErrorType
               && IsProceeded == other.IsProceeded
               && IsFailure == other.IsFailure;
    }
}
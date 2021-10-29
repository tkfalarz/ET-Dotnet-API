using System;
using System.Collections.Generic;

namespace ET.WebAPI.Kernel.ErrorsHandling
{
    public class OperationResult<T> : IEquatable<OperationResult<T>>
    {
        public string ErrorMessage { get; }
        public ErrorType ErrorType { get; }
        public bool IsProceeded { get; }
        public bool IsFailure => !IsProceeded;
        public T Value { get; }

        private OperationResult(bool isProceeded, string errorMessage, ErrorType errorType, T value)
        {
            if (errorMessage == null) throw new InvalidOperationException("Error message cannot be null");

            IsProceeded = isProceeded;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
            Value = value;
        }

        public static OperationResult<T> Proceeded(T value) => new(true, string.Empty, ErrorsHandling.ErrorType.None, value);
        public static OperationResult<T> Failure(string errorMessage, ErrorType errorType) => new(false, errorMessage, errorType, (T)default);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OperationResult<T>)obj);
        }

        public bool Equals(OperationResult<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ErrorMessage == other.ErrorMessage
                   && ErrorType == other.ErrorType 
                   && IsProceeded == other.IsProceeded 
                   && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ErrorMessage, (int)ErrorType, IsProceeded, Value);
        }
    }
}
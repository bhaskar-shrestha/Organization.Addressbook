using System;

namespace Organization.Addressbook.Api.Services
{
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public bool IsNotFound { get; protected set; }
        public string? Error { get; protected set; }

        protected Result() { }

        public static Result Success() => new Result { IsSuccess = true };
        public static Result NotFound() => new Result { IsSuccess = false, IsNotFound = true };
        public static Result Fail(string error) => new Result { IsSuccess = false, Error = error };
    }

    public class Result<T> : Result
    {
        public T? Value { get; private set; }

        protected Result() { }

        public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };
        public static new Result<T> NotFound() => new Result<T> { IsSuccess = false, IsNotFound = true };
        public static new Result<T> Fail(string error) => new Result<T> { IsSuccess = false, Error = error };
    }
}

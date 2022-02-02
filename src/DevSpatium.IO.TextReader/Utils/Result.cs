using System;

namespace DevSpatium.IO.TextReader
{
    internal class Result
    {
        protected Result() => Success = true;
        protected Result(string errorMessage) => ErrorMessage = errorMessage;

        protected Result(Exception exception)
        {
            ErrorMessage = exception?.Message;
            Exception = exception;
        }

        protected Result(Result result)
        {
            Success = result?.Success ?? false;
            ErrorMessage = result?.ErrorMessage;
            Exception = result?.Exception;
        }

        public bool Success { get; }
        public string ErrorMessage { get; }
        public Exception Exception { get; }
        public static Result Successful() => new Result();
        public static Result Failed(string errorMessage) => new Result(errorMessage);
        public static Result Failed(Exception exception) => new Result(exception);
    }

    internal sealed class Result<TData> : Result
    {
        private Result(TData data)
            => Data = data;

        private Result(string errorMessage)
            : base(errorMessage)
        {
        }

        private Result(string errorMessage, TData data)
            : base(errorMessage)
            => Data = data;

        private Result(Exception exception)
            : base(exception)
        {
        }

        private Result(Exception exception, TData data)
            : base(exception)
            => Data = data;

        private Result(Result result)
            : base(result)
        {
        }

        private Result(Result result, TData data)
            : base(result)
            => Data = data;

        public TData Data { get; }

        public static Result<TData> Successful(TData data) => new Result<TData>(data);
        public new static Result<TData> Failed(string errorMessage) => new Result<TData>(errorMessage);
        public static Result<TData> Failed(string errorMessage, TData data) => new Result<TData>(errorMessage, data);
        public new static Result<TData> Failed(Exception exception) => new Result<TData>(exception);
        public static Result<TData> Failed(Exception exception, TData data) => new Result<TData>(exception, data);
        public static Result<TData> Failed(Result result) => new Result<TData>(result);
        public static Result<TData> Failed(Result result, TData data) => new Result<TData>(result, data);
    }
}
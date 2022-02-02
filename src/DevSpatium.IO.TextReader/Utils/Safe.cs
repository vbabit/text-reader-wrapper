using System;

namespace DevSpatium.IO.TextReader
{
    internal static class Safe
    {
        public static Result<T> Execute<T>(Func<T> func)
        {
            try
            {
                var resultData = func.NotNull(nameof(func)).Invoke();
                return Result<T>.Successful(resultData);
            }
            catch (Exception exception)
            {
                Errors.FailDebug(exception.Message);
                return Result<T>.Failed(exception);
            }
        }

        public static Result Execute(Action action)
        {
            try
            {
                action.NotNull(nameof(action)).Invoke();
                return Result.Successful();
            }
            catch (Exception exception)
            {
                Errors.FailDebug(exception.Message);
                return Result.Failed(exception);
            }
        }
    }
}
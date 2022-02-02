using System;
using System.Diagnostics;

namespace DevSpatium.IO.TextReader
{
    internal static class Errors
    {
        public static Exception NullArgument(string parameterName)
        {
            var exception = new ArgumentNullException(parameterName);
            FailDebug(exception.Message);
            return exception;
        }

        public static Exception InvalidArgument(string message, string parameterName)
        {
            var exception = new ArgumentException(message, parameterName);
            FailDebug(exception.Message);
            return exception;
        }

        public static Exception InvalidOperation(string message)
        {
            var exception = new InvalidOperationException(message);
            FailDebug(exception.Message);
            return exception;
        }

        public static Exception NotSupportedType(Type type)
        {
            var exception = type != null
                ? new NotSupportedException(ErrorMessages.Common.NotSupported)
                : new NotSupportedException();
            FailDebug(exception.Message);
            return exception;
        }

        public static Exception DisposedObject(Type type)
        {
            var exception = new ObjectDisposedException(type?.Name);
            FailDebug(exception.Message);
            return exception;
        }

        public static Exception Validation(string message) => new ValidationException(message);
        public static Exception TextReader(string message) => new TextReaderException(message);
        public static Exception EndOfText() => new EndOfTextException();
        public static void FailDebug(string message) => Debug.Fail(message ?? ErrorMessages.Common.DebugFailure);
    }
}
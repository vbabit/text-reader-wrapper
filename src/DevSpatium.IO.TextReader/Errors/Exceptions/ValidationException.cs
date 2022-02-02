using System;
using static DevSpatium.IO.TextReader.ErrorMessages.Validation;

namespace DevSpatium.IO.TextReader
{
    public class ValidationException : Exception
    {
        public ValidationException()
            : base(GenericError)
        {
        }

        public ValidationException(string message)
            : base(!string.IsNullOrWhiteSpace(message) ? message : GenericError)
        {
        }

        public ValidationException(string message, Exception innerException)
            : base(!string.IsNullOrWhiteSpace(message) ? message : GenericError, innerException)
        {
        }
    }
}
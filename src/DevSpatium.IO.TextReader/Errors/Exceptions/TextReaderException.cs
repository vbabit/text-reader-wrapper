using System;
using static DevSpatium.IO.TextReader.ErrorMessages.Reader;

namespace DevSpatium.IO.TextReader
{
    public class TextReaderException : Exception
    {
        public TextReaderException()
            : base(GenericError)
        {
        }

        public TextReaderException(string message)
            : base(!string.IsNullOrWhiteSpace(message) ? message : GenericError)
        {
        }

        public TextReaderException(string message, Exception innerException)
            : base(!string.IsNullOrWhiteSpace(message) ? message : GenericError, innerException)
        {
        }
    }
}
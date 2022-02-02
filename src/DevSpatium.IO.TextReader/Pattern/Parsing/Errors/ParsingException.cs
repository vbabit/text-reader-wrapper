using System;
using static DevSpatium.IO.TextReader.ErrorMessages.Pattern;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    public class ParsingException : TextReaderException
    {
        public ParsingException()
            : base(GenericError)
        {
        }

        public ParsingException(string message)
            : base(!string.IsNullOrWhiteSpace(message) ? message : GenericError)
        {
        }

        public ParsingException(string message, Exception innerException)
            : base(!string.IsNullOrWhiteSpace(message) ? message : GenericError, innerException)
        {
        }
    }
}
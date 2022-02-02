using System;
using static DevSpatium.IO.TextReader.ErrorMessages.Reader;

namespace DevSpatium.IO.TextReader
{
    public class EndOfTextException : TextReaderException
    {
        public EndOfTextException()
            : base(EndOfText)
        {
        }

        public EndOfTextException(string message)
            : base(!string.IsNullOrWhiteSpace(message) ? message : EndOfText)
        {
        }

        public EndOfTextException(string message, Exception innerException)
            : base(!string.IsNullOrWhiteSpace(message) ? message : EndOfText, innerException)
        {
        }
    }
}
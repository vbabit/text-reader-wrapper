using System;

namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    internal sealed class DefaultExceptionWrapper : IExceptionHandler
    {
        public bool TryHandle(Exception originalException, out Exception finalException)
        {
            if (originalException == null)
            {
                finalException = new TextReaderException();
                return false;
            }

            var readerPosition = originalException.Data[InternalConstants.ReaderPositionExceptionDataKey];
            var message = originalException.Message;
            if (readerPosition is int readerPositionNumber)
            {
                message += $" {readerPositionNumber.ToInvariantString(ErrorMessages.Reader.Position)}";
            }

            finalException = new TextReaderException(message, originalException);
            return false;
        }
    }
}
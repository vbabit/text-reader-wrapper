using System;
using DevSpatium.IO.TextReader.Pattern.Parsing;

namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    internal sealed class DetailedExceptionWrapper : IExceptionHandler
    {
        private readonly IPatternChunkProvider _patternChunkProvider;

        public DetailedExceptionWrapper(IPatternChunkProvider patternChunkProvider)
            => _patternChunkProvider = patternChunkProvider.NotNull(nameof(patternChunkProvider));

        public bool TryHandle(Exception originalException, out Exception finalException)
        {
            if (originalException == null)
            {
                finalException = new TextReaderException();
                return false;
            }

            var operationHashCode = originalException.Data[InternalConstants.OperationHashCodeExceptionDataKey];
            var readerPosition = originalException.Data[InternalConstants.ReaderPositionExceptionDataKey];
            finalException = WrapException(originalException, operationHashCode, readerPosition);
            return false;
        }

        private Exception WrapException(Exception innerException, object operationHashCode, object readerPosition)
        {
            var message = innerException.Message;
            var patternChunk = GetPatternChunk(operationHashCode);
            if (!string.IsNullOrWhiteSpace(patternChunk.Pattern))
            {
                message += $" {patternChunk.Pattern.ToInvariantString(ErrorMessages.Reader.Operation)}";
            }

            var patternPosition = patternChunk.Position;
            if (patternPosition.HasValue)
            {
                message += $" {patternPosition.Value.ToInvariantString(ErrorMessages.Pattern.Position)}";
            }

            if (readerPosition is int readerPositionNumber)
            {
                message += $" {readerPositionNumber.ToInvariantString(ErrorMessages.Reader.Position)}";
            }

            return new TextReaderException(message, innerException);
        }

        private PatternChunk GetPatternChunk(object operationHashCode)
        {
            if (operationHashCode is int operationHashCodeNumber)
            {
                return Safe.Execute(() => _patternChunkProvider.GetChunk(operationHashCodeNumber)).Data;
            }

            return PatternChunk.Empty();
        }
    }
}
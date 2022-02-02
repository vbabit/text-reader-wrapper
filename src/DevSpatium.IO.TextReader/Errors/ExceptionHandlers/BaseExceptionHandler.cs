using System;

namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    internal abstract class BaseExceptionHandler : IExceptionHandler
    {
        private BaseExceptionHandler _nextHandler;

        public virtual bool TryHandle(Exception originalException, out Exception finalException)
        {
            if (_nextHandler != null)
            {
                return _nextHandler.TryHandle(originalException, out finalException);
            }

            finalException = originalException;
            return false;
        }

        public BaseExceptionHandler AssignNext(BaseExceptionHandler handler)
            => _nextHandler = handler.NotNull(nameof(handler));
    }
}
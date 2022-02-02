using System;

namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    internal sealed class DefaultExceptionHandler : BaseExceptionHandler
    {
        private readonly ExceptionType _exceptionType;
        private readonly OnException _behavior;
        private readonly IExceptionHandler _wrapper;
        private readonly IExceptionDataCleaner _cleaner;

        public DefaultExceptionHandler(
            OnException behavior,
            IExceptionHandler wrapper,
            IExceptionDataCleaner cleaner)
            : this(ExceptionType.From<Exception>(), behavior, wrapper, cleaner)
        {
        }

        public DefaultExceptionHandler(
            ExceptionType exceptionType,
            OnException behavior,
            IExceptionHandler wrapper,
            IExceptionDataCleaner cleaner)
        {
            _exceptionType = exceptionType.NotNull(nameof(exceptionType));
            _behavior = behavior.AssertDefined(nameof(behavior));
            _wrapper = wrapper.NotNull(nameof(wrapper));
            _cleaner = cleaner.NotNull(nameof(cleaner));
        }

        public override bool TryHandle(Exception originalException, out Exception finalException)
        {
            if (!_exceptionType.Type.IsAssignableFrom(originalException?.GetType()))
            {
                return base.TryHandle(originalException, out finalException);
            }

            try
            {
                switch (_behavior)
                {
                    case OnException.StopReading:
                        finalException = null;
                        return true;
                    case OnException.WrapAndThrow:
                        return _wrapper.TryHandle(originalException, out finalException);
                    default:
                        finalException = originalException;
                        return false;
                }
            }
            finally
            {
                _cleaner.CleanData(originalException);
            }
        }
    }
}
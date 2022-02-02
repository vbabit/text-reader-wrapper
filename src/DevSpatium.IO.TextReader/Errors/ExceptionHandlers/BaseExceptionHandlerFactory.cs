namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    internal abstract class BaseExceptionHandlerFactory : IExceptionHandlerFactory
    {
        private readonly IExceptionDataCleaner _cleaner = new ExceptionDataCleaner();
        private readonly IExceptionHandlingOptions _exceptionHandlingOptions;

        protected BaseExceptionHandlerFactory(IExceptionHandlingOptions exceptionHandlingOptions)
            => _exceptionHandlingOptions = exceptionHandlingOptions.NotNull(nameof(exceptionHandlingOptions));

        public IExceptionHandler Create()
        {
            var wrapper = CreateWrapper();
            BaseExceptionHandler chainOfHandlers = null;
            foreach (var option in _exceptionHandlingOptions)
            {
                var handler = new DefaultExceptionHandler(option.Key, option.Value, wrapper, _cleaner);
                if (chainOfHandlers != null)
                {
                    chainOfHandlers.AssignNext(handler);
                }
                else
                {
                    chainOfHandlers = handler;
                }
            }

            return chainOfHandlers ?? new DefaultExceptionHandler(OnException.Throw, wrapper, _cleaner);
        }

        protected abstract IExceptionHandler CreateWrapper();
    }
}
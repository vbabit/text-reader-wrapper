namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    internal sealed class DefaultExceptionHandlerFactory : BaseExceptionHandlerFactory
    {
        public DefaultExceptionHandlerFactory(IExceptionHandlingOptions exceptionHandlingOptions)
            : base(exceptionHandlingOptions)
        {
        }

        protected override IExceptionHandler CreateWrapper() => new DefaultExceptionWrapper();
    }
}
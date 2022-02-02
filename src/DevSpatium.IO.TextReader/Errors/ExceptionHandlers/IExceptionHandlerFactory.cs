namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    internal interface IExceptionHandlerFactory
    {
        IExceptionHandler Create();
    }
}
using System;

namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    internal interface IExceptionHandler
    {
        bool TryHandle(Exception originalException, out Exception finalException);
    }
}
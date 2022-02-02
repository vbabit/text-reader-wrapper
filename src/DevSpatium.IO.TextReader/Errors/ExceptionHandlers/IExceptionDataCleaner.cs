using System;

namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    internal interface IExceptionDataCleaner
    {
        void CleanData(Exception exception);
    }
}
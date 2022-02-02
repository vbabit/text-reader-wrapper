using System.Collections.Generic;

namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    internal interface IExceptionHandlingOptions : IReadOnlyDictionary<ExceptionType, OnException>
    {
    }
}
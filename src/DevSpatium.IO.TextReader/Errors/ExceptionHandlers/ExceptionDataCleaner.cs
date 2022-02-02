using System;

namespace DevSpatium.IO.TextReader.ExceptionHandlers
{
    internal sealed class ExceptionDataCleaner : IExceptionDataCleaner
    {
        public void CleanData(Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            Safe.Execute(() =>
            {
                if (exception.Data.Contains(InternalConstants.ReaderPositionExceptionDataKey))
                {
                    exception.Data.Remove(InternalConstants.ReaderPositionExceptionDataKey);
                }

                if (exception.Data.Contains(InternalConstants.OperationHashCodeExceptionDataKey))
                {
                    exception.Data.Remove(InternalConstants.OperationHashCodeExceptionDataKey);
                }
            });
        }
    }
}
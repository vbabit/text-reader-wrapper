using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader.Operations
{
    public abstract class BaseOperation : IOperation
    {
        protected BaseOperation(OperationType operationType)
            => OperationType = operationType.AssertDefined(nameof(operationType));

        public OperationType OperationType { get; }

        public virtual async Task ReadAsync(
            ITextReader reader,
            ICollection<string> output,
            CancellationToken cancelToken)
        {
            reader.NotNull(nameof(reader));
            output.NotNull(nameof(output));
            try
            {
                await ReadInternalAsync(reader, output, cancelToken).NonUi();
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                Safe.Execute(() =>
                {
                    exception.Data[InternalConstants.ReaderPositionExceptionDataKey] = reader.Position;
                    exception.Data[InternalConstants.OperationHashCodeExceptionDataKey] = GetHashCode();
                });
                throw;
            }
        }

        protected abstract Task ReadInternalAsync(
            ITextReader reader,
            ICollection<string> output,
            CancellationToken cancelToken);
    }
}
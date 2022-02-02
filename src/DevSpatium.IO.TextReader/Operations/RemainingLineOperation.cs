using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader.Operations
{
    public sealed class RemainingLineOperation : BaseOperation, IVisitableOperation
    {
        public RemainingLineOperation(OperationType operationType)
            : base(operationType)
        {
        }

        protected override async Task ReadInternalAsync(
            ITextReader reader,
            ICollection<string> output,
            CancellationToken cancelToken)
        {
            var result = await reader.ReadLineAsync(cancelToken).NonUi();
            if (result == null)
            {
                throw Errors.EndOfText();
            }

            if (OperationType == OperationType.Read)
            {
                output.Add(result);
            }
        }

        void IVisitableOperation.AcceptVisitor(IOperationVisitor visitor)
            => visitor.NotNull(nameof(visitor)).Visit(this);
    }
}
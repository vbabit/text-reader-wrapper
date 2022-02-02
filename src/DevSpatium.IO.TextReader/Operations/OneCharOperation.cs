using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader.Operations
{
    public sealed class OneCharOperation : BaseOperation, IVisitableOperation
    {
        public OneCharOperation(OperationType operationType)
            : base(operationType)
        {
        }

        protected override Task ReadInternalAsync(
            ITextReader reader,
            ICollection<string> output,
            CancellationToken cancelToken)
        {
            var result = reader.Read();
            if (result == null)
            {
                throw Errors.EndOfText();
            }

            if (OperationType == OperationType.Read)
            {
                output.Add(result.ToString());
            }

            return Task.FromResult((object)null);
        }

        void IVisitableOperation.AcceptVisitor(IOperationVisitor visitor)
            => visitor.NotNull(nameof(visitor)).Visit(this);
    }
}
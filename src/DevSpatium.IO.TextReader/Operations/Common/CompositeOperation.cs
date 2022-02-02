using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader.Operations
{
    public class CompositeOperation : IVisitableOperation
    {
        private readonly List<IOperation> _operations = new List<IOperation>();
        public IReadOnlyCollection<IOperation> Operations => _operations;

        public virtual async Task ReadAsync(
            ITextReader reader,
            ICollection<string> output,
            CancellationToken cancelToken)
        {
            reader.NotNull(nameof(reader));
            output.NotNull(nameof(output));
            foreach (var operation in _operations)
            {
                cancelToken.ThrowIfCancellationRequested();
                await operation.ReadAsync(reader, output, cancelToken).NonUi();
            }
        }

        public CompositeOperation Add(IOperation operation)
        {
            operation.NotNull(nameof(operation));
            _operations.Add(operation);
            return this;
        }

        void IVisitableOperation.AcceptVisitor(IOperationVisitor visitor)
            => visitor.NotNull(nameof(visitor)).Visit(this);
    }
}
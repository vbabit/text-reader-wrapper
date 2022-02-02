using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader.Operations
{
    public sealed class DefaultOperationBlock : BaseOperationBlock, IVisitableOperation
    {
        private const int DefaultNumberOfRepetitions = 1;

        public DefaultOperationBlock(
            IOperation body,
            int numberOrRepetitions = DefaultNumberOfRepetitions)
            : base(body)
            => NumberOfRepetitions = numberOrRepetitions.AssertMe(numberOrRepetitions > 0, nameof(numberOrRepetitions));

        public int NumberOfRepetitions { get; }

        public override async Task ReadAsync(
            ITextReader reader,
            ICollection<string> output,
            CancellationToken cancelToken)
        {
            reader.NotNull(nameof(reader));
            output.NotNull(nameof(output));
            for (var i = 0; i < NumberOfRepetitions; i++)
            {
                cancelToken.ThrowIfCancellationRequested();
                await Body.ReadAsync(reader, output, cancelToken).NonUi();
            }
        }

        void IVisitableOperation.AcceptVisitor(IOperationVisitor visitor)
            => visitor.NotNull(nameof(visitor)).Visit(this);
    }
}
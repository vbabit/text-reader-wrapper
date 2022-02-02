using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader.Operations
{
    public sealed class ContinuousOperationBlock : BaseOperationBlock, IVisitableOperation
    {
        public ContinuousOperationBlock(IOperation body)
            : base(body)
        {
        }

        public override async Task ReadAsync(
            ITextReader reader,
            ICollection<string> output,
            CancellationToken cancelToken)
        {
            reader.NotNull(nameof(reader));
            output.NotNull(nameof(output));
            while (!reader.EndOfText)
            {
                cancelToken.ThrowIfCancellationRequested();
                var initialPosition = reader.Position;
                await Body.ReadAsync(reader, output, cancelToken).NonUi();
                if (reader.Position == initialPosition)
                {
                    throw Errors.TextReader(ErrorMessages.Reader.InfiniteLoop);
                }
            }
        }

        void IVisitableOperation.AcceptVisitor(IOperationVisitor visitor)
            => visitor.NotNull(nameof(visitor)).Visit(this);
    }
}
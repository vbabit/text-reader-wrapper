using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader.Operations
{
    public sealed class CharBlockOperation : BaseOperation, IVisitableOperation
    {
        private const int DefaultBlockSize = 1;

        public CharBlockOperation(
            OperationType operationType,
            int blockSize = DefaultBlockSize)
            : base(operationType)
            => BlockSize = blockSize.AssertMe(blockSize > 0, nameof(blockSize));

        public int BlockSize { get; }

        protected override async Task ReadInternalAsync(
            ITextReader reader,
            ICollection<string> output,
            CancellationToken cancelToken)
        {
            var result = await reader.ReadBlockAsync(BlockSize, cancelToken).NonUi();
            if (result == null)
            {
                throw Errors.EndOfText();
            }

            if (OperationType == OperationType.Read)
            {
                output.Add(result);
            }

            if (result.Length < BlockSize)
            {
                throw Errors.EndOfText();
            }
        }

        void IVisitableOperation.AcceptVisitor(IOperationVisitor visitor)
            => visitor.NotNull(nameof(visitor)).Visit(this);
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevSpatium.IO.TextReader.Operations
{
    public sealed class BoundaryStringOperation : BaseOperation, IVisitableOperation
    {
        private const int InitialChunkSize = 16;
        private const int ChunkSizeRatio = 4;

        public BoundaryStringOperation(
            OperationType operationType,
            BoundaryStringOperationBehavior operationBehavior,
            IBoundaryStringSequence boundaryStrings)
            : base(operationType)
        {
            OperationBehavior = operationBehavior.AssertDefined(nameof(operationBehavior));
            BoundaryStrings = boundaryStrings.NotNull_ItemsNotNull_ToArray(nameof(boundaryStrings));
        }

        public BoundaryStringOperationBehavior OperationBehavior { get; }
        public IReadOnlyCollection<IBoundaryString> BoundaryStrings { get; }

        protected override async Task ReadInternalAsync(
            ITextReader reader,
            ICollection<string> output,
            CancellationToken cancelToken)
        {
            var currentChunkSize = InitialChunkSize;
            string currentChunk;
            BoundaryStringMatch successfulMatch;
            while (true)
            {
                currentChunk = await reader.PeekBlockAsync(currentChunkSize, cancelToken).NonUi();
                if (currentChunk == null)
                {
                    throw Errors.EndOfText();
                }

                successfulMatch = FindMatch(currentChunk, cancelToken);
                if (successfulMatch != null)
                {
                    break;
                }

                if (currentChunk.Length < currentChunkSize)
                {
                    throw Errors.EndOfText();
                }

                currentChunkSize *= ChunkSizeRatio;
            }

            await CompleteAsync(reader, output, currentChunk, successfulMatch).NonUi();
        }

        private BoundaryStringMatch FindMatch(string text, CancellationToken cancelToken) => BoundaryStrings
            .AsParallel()
            .WithCancellation(cancelToken)
            .Select(boundaryString => boundaryString.Match(text))
            .FirstOrDefault(match => match?.Success ?? false);

        private async Task CompleteAsync(
            ITextReader reader,
            ICollection<string> output,
            string peekedText,
            BoundaryStringMatch successfulMatch)
        {
            var resultLength = successfulMatch.Index;
            if (OperationBehavior == BoundaryStringOperationBehavior.WithOverstepping)
            {
                resultLength += successfulMatch.Length;
            }

            var offset = resultLength;
            if (OperationBehavior == BoundaryStringOperationBehavior.WithOversteppingFromCounterpart)
            {
                offset += successfulMatch.Length;
            }

            await reader.SetPositionAsync(reader.Position + offset).NonUi();
            var result = peekedText.Substring(0, resultLength);
            if (OperationType == OperationType.Read)
            {
                output.Add(result);
            }
            else if (resultLength < offset)
            {
                output.Add(successfulMatch.Value);
            }
        }

        void IVisitableOperation.AcceptVisitor(IOperationVisitor visitor)
            => visitor.NotNull(nameof(visitor)).Visit(this);
    }
}
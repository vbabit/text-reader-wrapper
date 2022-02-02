using System.Collections.Generic;
using DevSpatium.IO.TextReader.Operations;
using DevSpatium.IO.TextReader.Pattern.Building;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class PatternChunkProvider : IPatternPositionRegistry, IPatternChunkProvider
    {
        private const int MaxChunkSize = 50;
        private const string MaxChunkPostfix = "<...>";

        private readonly Dictionary<int, OperationMetadata> _registry = new Dictionary<int, OperationMetadata>();
        private readonly IPatternBuilder _patternBuilder;

        public PatternChunkProvider(IPatternBuilder patternBuilder)
            => _patternBuilder = patternBuilder.NotNull(nameof(patternBuilder));

        public void Register(IOperation operation, PatternPosition patternPosition)
        {
            operation.NotNull(nameof(operation));
            _registry.Add(operation.GetHashCode(), new OperationMetadata(operation, patternPosition));
        }

        public PatternChunk GetChunk(int operationHashCode)
        {
            if (!_registry.TryGetValue(operationHashCode, out var operationMetadata))
            {
                return PatternChunk.Empty();
            }

            var pattern = _patternBuilder.Build(operationMetadata.Operation);
            if (pattern.Length > MaxChunkSize)
            {
                pattern = pattern.Substring(0, MaxChunkSize) + MaxChunkPostfix;
            }

            return new PatternChunk(pattern, operationMetadata.PatternPosition);
        }

        public void Clear() => _registry.Clear();

        private sealed class OperationMetadata
        {
            public OperationMetadata(IOperation operation, PatternPosition patternPosition)
            {
                Operation = operation;
                PatternPosition = patternPosition;
            }

            public IOperation Operation { get; }
            public PatternPosition PatternPosition { get; }
        }
    }
}
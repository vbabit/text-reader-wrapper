using System.Linq;
using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal abstract class BaseOperationParser : IPatternParser
    {
        private readonly INumberParser _numberOfRepetitionsParser;
        private readonly IPatternPositionRegistry _patternPositionRegistry;

        private readonly char[] _supportedTokens =
        {
            Tokens.ReadOperation,
            Tokens.SkipOperation
        };

        private BaseOperationParser _nextParser;

        protected BaseOperationParser(
            INumberParser numberOfRepetitionsParser,
            IPatternPositionRegistry patternPositionRegistry)
        {
            _numberOfRepetitionsParser = numberOfRepetitionsParser.NotNull(nameof(numberOfRepetitionsParser));
            _patternPositionRegistry = patternPositionRegistry.NotNull(nameof(patternPositionRegistry));
        }

        public virtual bool CanParse(IPatternReader reader)
            => reader.NotNull(nameof(reader)).CheckNextToken(token => _supportedTokens.Contains(token));

        public virtual void Parse(IPatternReader reader, CompositeOperation operations)
        {
            reader.NotNull(nameof(reader));
            operations.NotNull(nameof(operations));
            var initialPosition = reader.Position;
            var operationType = ParseOperationType(reader);
            var operation = ParseOperation(reader, operationType);
            _patternPositionRegistry.Register(operation, initialPosition);
            var completeOperation = RepeatIfRequired(reader, operation);
            operations.Add(completeOperation);
        }

        public BaseOperationParser AssignNext(BaseOperationParser parser)
            => _nextParser = parser.NotNull(nameof(parser));

        protected virtual BaseOperation ParseOperation(IPatternReader reader, OperationType operationType)
        {
            reader.NotNull(nameof(reader));
            operationType.AssertDefined(nameof(operationType));
            if (_nextParser != null)
            {
                return _nextParser.ParseOperation(reader, operationType);
            }

            throw reader.FailNextToken();
        }

        private IOperation RepeatIfRequired(IPatternReader reader, IOperation operation)
        {
            if (!_numberOfRepetitionsParser.CanParse(reader))
            {
                return operation;
            }

            var numberOfRepetitions = _numberOfRepetitionsParser.Parse(reader);
            return new DefaultOperationBlock(operation, numberOfRepetitions);
        }

        private OperationType ParseOperationType(IPatternReader reader)
        {
            var operationToken = reader.ReadAndAssert(token => _supportedTokens.Contains(token));
            return operationToken == Tokens.ReadOperation ? OperationType.Read : OperationType.Skip;
        }
    }
}
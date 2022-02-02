using System.Linq;
using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class BoundaryStringOperationParser : BaseOperationParser
    {
        private readonly IBoundaryStringSequenceParser _stringSequenceParser;
        private readonly IOperationParameterParser _operationParameterParser;

        private readonly char[] _supportedTokens =
        {
            Tokens.BoundaryStringOperationNoOversteppingQualifier,
            Tokens.BoundaryStringOperationWithOversteppingQualifier
        };

        public BoundaryStringOperationParser(
            IBoundaryStringSequenceParser stringSequenceParser,
            IOperationParameterParser operationParameterParser,
            INumberParser numberOfRepetitionsParser,
            IPatternPositionRegistry patternPositionRegistry)
            : base(numberOfRepetitionsParser, patternPositionRegistry)
        {
            _stringSequenceParser = stringSequenceParser.NotNull(nameof(stringSequenceParser));
            _operationParameterParser = operationParameterParser.NotNull(nameof(operationParameterParser));
        }

        protected override BaseOperation ParseOperation(IPatternReader reader, OperationType operationType)
        {
            if (!reader.CheckNextToken(token => _supportedTokens.Contains(token)))
            {
                return base.ParseOperation(reader, operationType);
            }

            var qualifyingToken = reader.Read();
            var boundaryStrings = _stringSequenceParser.Parse(reader);
            var operationBehavior = ParseOperationBehavior(reader, operationType, qualifyingToken);
            return new BoundaryStringOperation(operationType, operationBehavior, boundaryStrings);
        }

        private BoundaryStringOperationBehavior ParseOperationBehavior(
            IPatternReader reader,
            OperationType operationType,
            char qualifyingToken)
        {
            if (qualifyingToken == Tokens.BoundaryStringOperationWithOversteppingQualifier)
            {
                return BoundaryStringOperationBehavior.WithOverstepping;
            }

            if (!_operationParameterParser.CanParse(reader))
            {
                return BoundaryStringOperationBehavior.NoOverstepping;
            }

            var parameter = _operationParameterParser.Parse(reader);
            if (operationType == OperationType.Read && parameter == Tokens.SkipOperation ||
                operationType == OperationType.Skip && parameter == Tokens.ReadOperation)
            {
                return BoundaryStringOperationBehavior.WithOversteppingFromCounterpart;
            }

            throw reader.FailNextToken();
        }
    }
}
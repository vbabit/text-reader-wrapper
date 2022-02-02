using System.Linq;
using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal abstract class BaseOperationBlockParser : IPatternParser
    {
        private readonly IPatternScope _innerScope = new DefaultOperationBlockScope();
        private readonly IPatternParser _operationSequenceParser;
        private BaseOperationBlockParser _nextParser;

        protected BaseOperationBlockParser(IPatternParser operationSequenceParser)
            => _operationSequenceParser = operationSequenceParser.NotNull(nameof(operationSequenceParser));

        public virtual bool CanParse(IPatternReader reader) => _innerScope.CanEnter(reader);

        public virtual void Parse(IPatternReader reader, CompositeOperation operations)
        {
            reader.NotNull(nameof(reader));
            operations.NotNull(nameof(operations));
            var operationSequence = new CompositeOperation();
            var innerScope = _innerScope.Enter(reader);
            var initialPosition = reader.Position;
            _operationSequenceParser.Parse(reader, operationSequence);
            if (operationSequence.Operations.Count == 0)
            {
                throw ParsingErrors.EmptyOperationBlock(initialPosition);
            }

            innerScope.Exit(reader);
            var operationBlockBody = operationSequence.Operations.Count > 1
                ? operationSequence
                : operationSequence.Operations.First();
            var operationBlock = Complete(reader, operationBlockBody);
            operations.Add(operationBlock);
        }

        public BaseOperationBlockParser AssignNext(BaseOperationBlockParser parser)
            => _nextParser = parser.NotNull(nameof(parser));

        protected virtual IOperation Complete(IPatternReader reader, IOperation operationBlockBody)
        {
            reader.NotNull(nameof(reader));
            operationBlockBody.NotNull(nameof(operationBlockBody));
            if (_nextParser != null)
            {
                return _nextParser.Complete(reader, operationBlockBody);
            }

            throw reader.FailNextToken();
        }
    }
}
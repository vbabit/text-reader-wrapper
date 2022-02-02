using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class DefaultOperationBlockParser : BaseOperationBlockParser
    {
        private readonly INumberParser _numberOfRepetitionsParser;

        public DefaultOperationBlockParser(
            IPatternParser operationSequenceParser,
            INumberParser numberOfRepetitionsParser)
            : base(operationSequenceParser)
            => _numberOfRepetitionsParser = numberOfRepetitionsParser.NotNull(nameof(numberOfRepetitionsParser));

        protected override IOperation Complete(IPatternReader reader, IOperation operationBlockBody)
        {
            if (!_numberOfRepetitionsParser.CanParse(reader))
            {
                return operationBlockBody;
            }

            var numberOfRepetitions = _numberOfRepetitionsParser.Parse(reader);
            if (numberOfRepetitions == 1)
            {
                return operationBlockBody;
            }

            return new DefaultOperationBlock(operationBlockBody, numberOfRepetitions);
        }
    }
}
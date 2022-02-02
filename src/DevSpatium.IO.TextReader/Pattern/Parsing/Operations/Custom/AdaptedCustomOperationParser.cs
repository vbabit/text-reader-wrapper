using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class AdaptedCustomOperationParser : BaseOperationParser
    {
        private readonly ICustomOperationParser _customParser;

        public AdaptedCustomOperationParser(
            ICustomOperationParser customParser,
            INumberParser numberOfRepetitionsParser,
            IPatternPositionRegistry patternPositionRegistry)
            : base(numberOfRepetitionsParser, patternPositionRegistry)
            => _customParser = customParser.NotNull(nameof(customParser));

        public override bool CanParse(IPatternReader reader)
            => reader.NotNull(nameof(reader)).CheckNextToken(token => _customParser.IsOperationSupported(token));

        protected override BaseOperation ParseOperation(IPatternReader reader, OperationType operationType)
        {
            if (!reader.CheckNextToken(token => _customParser.IsOperationSupported(token)))
            {
                return base.ParseOperation(reader, operationType);
            }

            return _customParser.Parse(reader, operationType);
        }
    }
}
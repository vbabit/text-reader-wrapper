using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class CharBlockOperationParser : BaseOperationParser
    {
        private readonly INumberParser _blockSizeParser;

        public CharBlockOperationParser(
            INumberParser blockLengthParser,
            INumberParser numberOfRepetitionsParser,
            IPatternPositionRegistry patternPositionRegistry)
            : base(numberOfRepetitionsParser, patternPositionRegistry)
            => _blockSizeParser = blockLengthParser.NotNull(nameof(blockLengthParser));

        protected override BaseOperation ParseOperation(IPatternReader reader, OperationType operationType)
        {
            if (!_blockSizeParser.CanParse(reader))
            {
                return base.ParseOperation(reader, operationType);
            }

            var blockSize = _blockSizeParser.Parse(reader);
            return new CharBlockOperation(operationType, blockSize);
        }
    }
}
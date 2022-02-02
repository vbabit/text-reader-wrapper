using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class RemainingLineOperationParser : BaseOperationParser
    {
        public RemainingLineOperationParser(
            INumberParser numberOfRepetitionsParser,
            IPatternPositionRegistry patternPositionRegistry)
            : base(numberOfRepetitionsParser, patternPositionRegistry)
        {
        }

        protected override BaseOperation ParseOperation(IPatternReader reader, OperationType operationType)
        {
            if (!reader.CheckNextToken(Tokens.RemainingLineOperationQualifier))
            {
                return base.ParseOperation(reader, operationType);
            }

            reader.Read(); // qualifying token
            return new RemainingLineOperation(operationType);
        }
    }
}
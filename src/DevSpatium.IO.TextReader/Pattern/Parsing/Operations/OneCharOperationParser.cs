using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class OneCharOperationParser : BaseOperationParser
    {
        public OneCharOperationParser(
            INumberParser numberOfRepetitionsParser,
            IPatternPositionRegistry patternPositionRegistry)
            : base(numberOfRepetitionsParser, patternPositionRegistry)
        {
        }

        protected override BaseOperation ParseOperation(IPatternReader reader, OperationType operationType)
        {
            if (!reader.CheckNextToken(Tokens.OneCharOperationQualifier))
            {
                return base.ParseOperation(reader, operationType);
            }

            reader.Read(); // qualifying token
            return new OneCharOperation(operationType);
        }
    }
}
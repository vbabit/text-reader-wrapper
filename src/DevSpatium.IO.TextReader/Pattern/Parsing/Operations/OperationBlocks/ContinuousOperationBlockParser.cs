using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class ContinuousOperationBlockParser : BaseOperationBlockParser
    {
        public ContinuousOperationBlockParser(IPatternParser operationSequenceParser)
            : base(operationSequenceParser)
        {
        }

        protected override IOperation Complete(IPatternReader reader, IOperation operationSequence)
        {
            if (!reader.CheckNextToken(Tokens.ContinuousOperationBlockPostfix))
            {
                return base.Complete(reader, operationSequence);
            }

            var positionBeforePostfix = reader.Position;
            reader.Read(); // postfix
            if (!reader.EndOfPattern)
            {
                throw ParsingErrors.UnexpectedToken(Tokens.ContinuousOperationBlockPostfix, positionBeforePostfix);
            }

            return new ContinuousOperationBlock(operationSequence);
        }
    }
}
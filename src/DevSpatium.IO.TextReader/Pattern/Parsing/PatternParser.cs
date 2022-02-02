using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class PatternParser : CompositePatternParser
    {
        public override void Parse(IPatternReader reader, CompositeOperation operations)
        {
            reader.NotNull(nameof(reader));
            operations.NotNull(nameof(operations));
            base.Parse(reader, operations);
            if (!reader.EndOfPattern)
            {
                throw reader.FailNextToken();
            }
        }
    }
}
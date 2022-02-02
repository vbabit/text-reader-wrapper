namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class RegexBoundaryStringScope : PatternScope
    {
        public RegexBoundaryStringScope()
            : base(Tokens.RegexBoundaryStringQuotationMark, Tokens.RegexBoundaryStringQuotationMark)
        {
        }
    }
}
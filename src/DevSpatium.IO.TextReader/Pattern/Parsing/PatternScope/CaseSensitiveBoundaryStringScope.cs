namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class CaseSensitiveBoundaryStringScope : PatternScope
    {
        public CaseSensitiveBoundaryStringScope()
            : base(Tokens.CaseSensitiveBoundaryStringQuotationMark, Tokens.CaseSensitiveBoundaryStringQuotationMark)
        {
        }
    }
}
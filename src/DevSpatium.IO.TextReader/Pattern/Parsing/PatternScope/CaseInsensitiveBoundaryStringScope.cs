namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class CaseInsensitiveBoundaryStringScope : PatternScope
    {
        public CaseInsensitiveBoundaryStringScope()
            : base(Tokens.CaseInsensitiveBoundaryStringQuotationMark, Tokens.CaseInsensitiveBoundaryStringQuotationMark)
        {
        }
    }
}
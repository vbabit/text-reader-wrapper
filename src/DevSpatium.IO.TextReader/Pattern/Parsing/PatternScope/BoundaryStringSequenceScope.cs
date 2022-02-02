namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class BoundaryStringSequenceScope : PatternScope
    {
        public BoundaryStringSequenceScope()
            : base(Tokens.BoundaryStringSequenceScopeBeginning, Tokens.BoundaryStringSequenceScopeEnding)
        {
        }
    }
}
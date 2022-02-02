namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class CharBlockSizeScope : PatternScope
    {
        public CharBlockSizeScope()
            : base(Tokens.CharBlockSizeScopeBeginning, Tokens.CharBlockSizeScopeEnding)
        {
        }
    }
}
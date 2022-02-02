namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class NumberOfRepetitionsScope : PatternScope
    {
        public NumberOfRepetitionsScope()
            : base(Tokens.NumberOfRepetitionsScopeBeginning, Tokens.NumberOfRepetitionsScopeEnding)
        {
        }
    }
}
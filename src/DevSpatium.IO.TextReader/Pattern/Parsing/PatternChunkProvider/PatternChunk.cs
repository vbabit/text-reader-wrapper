namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class PatternChunk
    {
        private PatternChunk() => Pattern = string.Empty;

        public PatternChunk(string pattern, PatternPosition position)
        {
            Pattern = pattern.NotNullOrBlank(nameof(pattern));
            Position = position;
        }

        public string Pattern { get; }
        public PatternPosition? Position { get; }

        public static PatternChunk Empty() => new PatternChunk();
        public override string ToString() => Pattern;
    }
}
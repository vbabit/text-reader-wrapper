namespace DevSpatium.IO.TextReader
{
    public sealed class ReaderOptions
    {
        private TextComparison _textComparison = TextComparison.IgnoreCulture;

        public TextComparison TextComparison
        {
            get => _textComparison;
            set => _textComparison = value.AssertDefined();
        }
    }
}
namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class CharBlockSizeParser : BaseNumberParser
    {
        private readonly IPatternScope _innerScope = new CharBlockSizeScope();

        public CharBlockSizeParser()
            : base(NumberOptions.Positive)
        {
        }

        public override bool CanParse(IPatternReader reader) => _innerScope.CanEnter(reader);
        protected override IPatternScope EnterInnerScope(IPatternReader reader) => _innerScope.Enter(reader);
    }
}
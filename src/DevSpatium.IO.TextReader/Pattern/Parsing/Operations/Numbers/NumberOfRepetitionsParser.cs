namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class NumberOfRepetitionsParser : BaseNumberParser
    {
        private readonly IPatternScope _innerScope = new NumberOfRepetitionsScope();

        public NumberOfRepetitionsParser()
            : base(NumberOptions.Positive)
        {
        }

        public override bool CanParse(IPatternReader reader) => _innerScope.CanEnter(reader);
        protected override IPatternScope EnterInnerScope(IPatternReader reader) => _innerScope.Enter(reader);
    }
}
using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class CaseInsensitiveBoundaryStringParser : BaseBoundaryStringParser
    {
        private readonly IPatternScope _innerScope = new CaseInsensitiveBoundaryStringScope();
        private readonly TextComparison _textComparison;

        public CaseInsensitiveBoundaryStringParser(TextComparison textComparison)
            : base(CreateEscapeOptions())
            => _textComparison = textComparison.AssertDefined(nameof(textComparison));

        public override bool CanParse(IPatternReader reader) => _innerScope.CanEnter(reader);
        protected override IPatternScope EnterInnerScope(IPatternReader reader) => _innerScope.Enter(reader);

        protected override IBoundaryString CreateBoundaryString(string value, PatternPosition position)
            => new DefaultBoundaryString(value, true, _textComparison);

        private static EscapeOptions CreateEscapeOptions() => new EscapeOptions
        {
            NonLiteralTokens = new[]
            {
                new EscapeOptions.NonLiteralToken(Tokens.Escape),
                new EscapeOptions.NonLiteralToken(Tokens.CaseInsensitiveBoundaryStringQuotationMark),
                new EscapeOptions.NonLiteralToken(Tokens.CarriageReturn, InternalConstants.CarriageReturn),
                new EscapeOptions.NonLiteralToken(Tokens.LineFeed, InternalConstants.LineFeed)
            },
            ThrowIfEscapeSequenceUnrecognized = true
        };
    }
}
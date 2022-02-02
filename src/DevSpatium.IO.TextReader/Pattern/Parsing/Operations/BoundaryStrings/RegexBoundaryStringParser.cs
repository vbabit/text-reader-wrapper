using System;
using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class RegexBoundaryStringParser : BaseBoundaryStringParser
    {
        private readonly IPatternScope _innerScope = new RegexBoundaryStringScope();
        private readonly TextComparison _textComparison;

        public RegexBoundaryStringParser(TextComparison textComparison)
            : base(CreateEscapeOptions())
            => _textComparison = textComparison.AssertDefined(nameof(textComparison));

        public override bool CanParse(IPatternReader reader) => _innerScope.CanEnter(reader);
        protected override IPatternScope EnterInnerScope(IPatternReader reader) => _innerScope.Enter(reader);

        protected override IBoundaryString CreateBoundaryString(string value, PatternPosition position)
        {
            try
            {
                return new RegexBoundaryString(value, _textComparison);
            }
            catch (ArgumentException exception)
            {
                throw ParsingErrors.InvalidRegex(exception.Message, position);
            }
        }

        private static EscapeOptions CreateEscapeOptions() => new EscapeOptions
        {
            NonLiteralTokens = new[]
            {
                new EscapeOptions.NonLiteralToken(Tokens.RegexBoundaryStringQuotationMark)
            }
        };
    }
}
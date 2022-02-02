using System.Collections.Generic;
using System.Linq;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class EscapeOptions
    {
        private NonLiteralToken[] _nonLiteralTokens;
        public char EscapeToken { get; set; } = Tokens.Escape;

        public IReadOnlyCollection<NonLiteralToken> NonLiteralTokens
        {
            get => _nonLiteralTokens ?? (_nonLiteralTokens = new NonLiteralToken[0]);
            set => _nonLiteralTokens = value.NotNull().ToArray();
        }

        public bool ThrowIfEscapeSequenceUnrecognized { get; set; }

        public sealed class NonLiteralToken
        {
            public NonLiteralToken(char value)
            {
                Value = value;
                MapTo = value;
            }

            public NonLiteralToken(char value, char mapTo)
            {
                Value = value;
                MapTo = mapTo;
            }

            public char Value { get; }
            public char MapTo { get; }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal abstract class BaseBoundaryStringParser : IBoundaryStringParser
    {
        protected BaseBoundaryStringParser(EscapeOptions escapeOptions)
            => EscapeOptions = escapeOptions.NotNull(nameof(escapeOptions));

        protected EscapeOptions EscapeOptions { get; }
        public abstract bool CanParse(IPatternReader reader);

        public virtual IBoundaryString Parse(IPatternReader reader)
        {
            reader.NotNull(nameof(reader));
            var innerScope = EnterInnerScope(reader).NotNull();
            var initialPosition = reader.Position;
            var chars = new List<char>();
            var nextToken = reader.PeekRaw();
            while (nextToken != innerScope.ClosingToken)
            {
                var currentToken = ReadChar(reader);
                chars.Add(currentToken);
                nextToken = reader.PeekRaw();
            }

            if (chars.Count == 0)
            {
                throw ParsingErrors.EmptyBoundaryString(initialPosition);
            }

            var boundaryString = CreateBoundaryString(new string(chars.ToArray()), initialPosition);
            innerScope.Exit(reader);
            return boundaryString;
        }

        protected abstract IPatternScope EnterInnerScope(IPatternReader reader);
        protected abstract IBoundaryString CreateBoundaryString(string value, PatternPosition position);

        private char ReadChar(IPatternReader reader)
        {
            var firstToken = reader.ReadRaw();
            if (EscapeOptions.NonLiteralTokens.Count == 0 || firstToken != EscapeOptions.EscapeToken)
            {
                return firstToken;
            }

            var secondToken = reader.PeekRaw();
            var nonLiteralToken = EscapeOptions.NonLiteralTokens.FirstOrDefault(token => token.Value == secondToken);
            if (nonLiteralToken != null)
            {
                reader.ReadRaw(); // peeked second token
                return nonLiteralToken.MapTo;
            }

            if (EscapeOptions.ThrowIfEscapeSequenceUnrecognized)
            {
                throw ParsingErrors.UnrecognizedEscapeSequence($"{firstToken}{secondToken}", reader.Position);
            }

            return firstToken;
        }
    }
}
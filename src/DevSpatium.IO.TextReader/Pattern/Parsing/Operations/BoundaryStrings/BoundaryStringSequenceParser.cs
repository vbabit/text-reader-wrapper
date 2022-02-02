using System.Collections.Generic;
using System.Linq;
using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class BoundaryStringSequenceParser : IBoundaryStringSequenceParser
    {
        private readonly IPatternScope _innerScope = new BoundaryStringSequenceScope();
        private readonly IBoundaryStringParser[] _stringParsers;

        public BoundaryStringSequenceParser(params IBoundaryStringParser[] stringParsers)
            => _stringParsers = stringParsers.NotNullOrEmpty_ItemsNotNull_ToArray(nameof(stringParsers));

        public bool CanParse(IPatternReader reader)
            => _innerScope.CanEnter(reader) || _stringParsers.Any(parser => parser.CanParse(reader));

        public IBoundaryStringSequence Parse(IPatternReader reader)
        {
            reader.NotNull(nameof(reader));
            if (!_innerScope.CanEnter(reader))
            {
                var boundaryString = GetParser(reader).Parse(reader);
                return new BoundaryStringSequence(boundaryString);
            }

            _innerScope.Enter(reader);
            if (reader.Peek() == _innerScope.ClosingToken)
            {
                throw ParsingErrors.EmptyBoundaryStringSequence(reader.Position);
            }

            var boundaryStrings = new List<IBoundaryString>();
            while (true)
            {
                var boundaryString = GetParser(reader).Parse(reader);
                boundaryStrings.Add(boundaryString);
                if (reader.Peek() == _innerScope.ClosingToken)
                {
                    break;
                }

                SkipSeparator(reader);
            }

            _innerScope.Exit(reader);
            return new BoundaryStringSequence(boundaryStrings);
        }

        private IBoundaryStringParser GetParser(IPatternReader reader)
        {
            var result = _stringParsers.FirstOrDefault(parser => parser.CanParse(reader));
            if (result != null)
            {
                return result;
            }

            throw reader.FailNextToken();
        }

        private void SkipSeparator(IPatternReader reader)
        {
            reader.ReadAndAssert(Tokens.BoundaryStringSeparator);
            if (reader.Peek() == _innerScope.ClosingToken)
            {
                throw reader.FailNextToken();
            }
        }
    }
}
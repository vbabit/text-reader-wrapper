namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal class PatternScope : IPatternScope
    {
        public PatternScope(char openingToken, char closingTokens)
        {
            OpeningToken = openingToken;
            ClosingToken = closingTokens;
        }

        public char OpeningToken { get; }
        public char ClosingToken { get; }

        public virtual bool CanEnter(IPatternReader reader)
            => reader.NotNull(nameof(reader)).CheckNextToken(OpeningToken);

        public virtual IPatternScope Enter(IPatternReader reader)
        {
            reader.NotNull(nameof(reader));
            reader.ReadAndAssert(OpeningToken);
            return this;
        }

        public virtual void Exit(IPatternReader reader)
        {
            reader.NotNull(nameof(reader));
            var nextToken = reader.Peek();
            if (!nextToken.HasValue)
            {
                throw ParsingErrors.MissingEndOfScope(ClosingToken);
            }

            if (nextToken.Value != ClosingToken)
            {
                throw ParsingErrors.UnexpectedToken(nextToken.Value, reader.Position);
            }

            reader.Read(); // peeked closing token
        }
    }
}
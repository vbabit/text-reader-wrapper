using System.Collections.Generic;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal abstract class BaseNumberParser : INumberParser
    {
        protected BaseNumberParser(NumberOptions numberOptions)
            => NumberOptions = numberOptions.AssertDefined(nameof(numberOptions));

        protected NumberOptions NumberOptions { get; }
        public abstract bool CanParse(IPatternReader reader);

        public int Parse(IPatternReader reader)
        {
            reader.NotNull(nameof(reader));
            var innerScope = EnterInnerScope(reader).NotNull();
            var initialPosition = reader.Position;
            var digits = new List<char>();
            var token = reader.Peek();
            if (token == Tokens.Plus || token == Tokens.Minus)
            {
                reader.ReadRaw(); // peeked +/-
                digits.Add(token.Value);
                token = reader.PeekRaw();
            }

            while (token.HasValue && char.IsDigit(token.Value))
            {
                reader.ReadRaw(); // peeked digit
                digits.Add(token.Value);
                token = reader.PeekRaw();
            }

            innerScope.Exit(reader);
            var numberString = new string(digits.ToArray());
            var number = ParseInteger(numberString, initialPosition);
            return number;
        }

        protected abstract IPatternScope EnterInnerScope(IPatternReader reader);

        private int ParseInteger(string value, PatternPosition position)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw ParsingErrors.MissingNumber(position);
            }

            if (!Helpers.TryParseToIntInvariant(value, out var number))
            {
                throw ParsingErrors.TooLargeNumber(int.MaxValue, position);
            }

            if (number == 0 && (NumberOptions & NumberOptions.Zero) == 0)
            {
                throw ParsingErrors.ZeroNumber(position);
            }

            if (number < 0 && (NumberOptions & NumberOptions.Negative) == 0)
            {
                throw ParsingErrors.NegativeNumber(position);
            }

            if (number > 0 && (NumberOptions & NumberOptions.Positive) == 0)
            {
                throw ParsingErrors.PositiveNumber(position);
            }

            return number;
        }
    }
}
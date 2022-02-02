using System;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    public interface IPatternReader : IDisposable
    {
        /// <summary>
        ///     The current position of the pattern.
        /// </summary>
        PatternPosition Position { get; }

        /// <summary>
        ///     Returns <c>true</c> if the end of the pattern has already been reached, otherwise: <c>false</c>.
        /// </summary>
        bool EndOfPattern { get; }

        /// <summary>
        ///     Skips any whitespace and peeks the next token.
        ///     If the end of the pattern has already been reached, returns <c>null</c>.
        /// </summary>
        char? Peek();

        /// <summary>
        ///     Skips any whitespace and reads the next token.
        ///     If the end of the pattern has already been reached, throws <see cref="ParsingException" />.
        /// </summary>
        char Read();

        /// <summary>
        ///     Peeks the token located directly next to the current position (does not skip whitespace).
        ///     If the end of the pattern has already been reached, returns <c>null</c>.
        /// </summary>
        char? PeekRaw();

        /// <summary>
        ///     Reads the token located directly next to the current position (does not skip whitespace).
        ///     If the end of the pattern has already been reached, throws <see cref="ParsingException" />.
        /// </summary>
        char ReadRaw();
    }

    public static class PatternReaderExtensions
    {
        /// <summary>
        ///     Skips any whitespace, reads the next token,
        ///     and validates it using <see cref="validationFunc" />.
        ///     If the validation fails, throws <see cref="ParsingException" />
        ///     containing details about the failed token.
        /// </summary>
        public static char ReadAndAssert(this IPatternReader reader, Func<char, bool> validationFunc)
        {
            reader.NotNull(nameof(reader));
            validationFunc.NotNull(nameof(validationFunc));
            var initialPosition = reader.Position;
            var token = reader.Read();
            if (validationFunc(token))
            {
                return token;
            }

            throw ParsingErrors.UnexpectedToken(token, initialPosition);
        }

        /// <summary>
        ///     Skips any whitespace, reads the next token,
        ///     and checks if the token is equal to <see cref="expectedToken" />.
        ///     If the validation fails, throws <see cref="ParsingException" />
        ///     containing details about the failed token.
        /// </summary>
        public static char ReadAndAssert(this IPatternReader reader, char expectedToken)
        {
            reader.NotNull(nameof(reader));
            var initialPosition = reader.Position;
            var token = reader.Read();
            if (token == expectedToken)
            {
                return token;
            }

            throw ParsingErrors.MissingToken(expectedToken, initialPosition);
        }

        /// <summary>
        ///     Skips any whitespace, reads the next token,
        ///     and validates it using <see cref="validationFunc" />.
        ///     Returns <c>true</c> if the validation succeeds, otherwise: <c>false</c>.
        /// </summary>
        public static bool CheckNextToken(this IPatternReader reader, Func<char, bool> validationFunc)
        {
            reader.NotNull(nameof(reader));
            validationFunc.NotNull(nameof(validationFunc));
            var nextToken = reader.Peek();
            return nextToken.HasValue && validationFunc(nextToken.Value);
        }

        /// <summary>
        ///     Skips any whitespace, reads the next token,
        ///     and checks if the token is equal to <see cref="expectedToken" />.
        ///     Returns <c>true</c> if the validation succeeds, otherwise: <c>false</c>.
        /// </summary>
        public static bool CheckNextToken(this IPatternReader reader, char expectedToken)
            => reader.NotNull(nameof(reader)).Peek() == expectedToken;

        /// <summary>
        ///     Throws <see cref="ParsingException" /> with details about the next token.
        /// </summary>
        public static Exception FailNextToken(this IPatternReader reader)
        {
            reader.NotNull(nameof(reader));
            var nextToken = reader.Peek();
            if (nextToken.HasValue)
            {
                throw ParsingErrors.UnexpectedToken(nextToken.Value, reader.Position);
            }

            throw ParsingErrors.UnexpectedEndOfPattern();
        }
    }
}
using System;
using Messages = DevSpatium.IO.TextReader.Pattern.Parsing.ParsingErrorMessages;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal static class ParsingErrors
    {
        public static Exception UnexpectedToken(char token, PatternPosition position)
            => new ParsingException(Helpers.ToInvariantString(Messages.UnexpectedToken, token, position));

        public static Exception MissingToken(char token, PatternPosition position)
            => new ParsingException(Helpers.ToInvariantString(Messages.MissingToken, token, position));

        public static Exception EmptyOperationBlock(PatternPosition position)
            => new ParsingException(position.ToInvariantString(Messages.EmptyOperationBlock));

        public static Exception EmptyBoundaryStringSequence(PatternPosition position)
            => new ParsingException(position.ToInvariantString(Messages.EmptyBoundaryStringSequence));

        public static Exception EmptyBoundaryString(PatternPosition position)
            => new ParsingException(position.ToInvariantString(Messages.EmptyBoundaryString));

        public static Exception MissingNumber(PatternPosition position)
            => new ParsingException(position.ToInvariantString(Messages.MissingNumber));

        public static Exception TooLargeNumber(int number, PatternPosition position)
            => new ParsingException(Helpers.ToInvariantString(Messages.TooLargeNumber, number, position));

        public static Exception NegativeNumber(PatternPosition position)
            => new ParsingException(position.ToInvariantString(Messages.NegativeNumber));

        public static Exception PositiveNumber(PatternPosition position)
            => new ParsingException(position.ToInvariantString(Messages.PositiveNumber));

        public static Exception ZeroNumber(PatternPosition position)
            => new ParsingException(position.ToInvariantString(Messages.ZeroNumber));

        public static Exception UnrecognizedEscapeSequence(string sequence, PatternPosition position)
            => new ParsingException(Helpers.ToInvariantString(Messages.UnrecognizedEscapeSequence, sequence, position));

        public static Exception InvalidRegex(string message, PatternPosition position)
            => new ParsingException(Helpers.ToInvariantString(Messages.InvalidRegex, message, position));

        public static Exception MissingOperationParameter(PatternPosition position)
            => new ParsingException(position.ToInvariantString(Messages.MissingOperationParameter));

        public static Exception MissingEndOfScope(char closingToken)
            => new ParsingException(closingToken.ToInvariantString(Messages.MissingEndOfScope));

        public static Exception UnexpectedEndOfPattern()
            => new ParsingException(Messages.UnexpectedEndOfPattern);

        public static Exception DisposedPatternReader() => Errors.DisposedObject(typeof(PatternReader));
    }
}
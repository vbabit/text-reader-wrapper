namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal static class ParsingErrorMessages
    {
        public static readonly string UnexpectedToken =
            "Unexpected token: \"{0}\". Position: {1}.";

        public static readonly string MissingToken =
            "Missing token: \"{0}\". Position: {1}.";

        public static readonly string EmptyOperationBlock =
            "Operation block is empty. Position: {0}.";

        public static readonly string EmptyBoundaryStringSequence =
            "Sequence of boundary strings is empty. Position: {0}.";

        public static readonly string EmptyBoundaryString =
            "Boundary string is empty. Position: {0}.";

        public static readonly string MissingNumber =
            "Number is missing. Position: {0}.";

        public static readonly string TooLargeNumber =
            "Number exceeds max value: {0}. Position: {1}.";

        public static readonly string NegativeNumber =
            "Number cannot be negative. Position: {0}.";

        public static readonly string PositiveNumber =
            "Number cannot be positive. Position: {0}.";

        public static readonly string ZeroNumber =
            "Number cannot be zero. Position: {0}.";

        public static readonly string UnrecognizedEscapeSequence =
            "Unrecognized escape sequence: \"{0}\". Position {1}.";

        public static readonly string InvalidRegex =
            "Invalid regular expression. {0} Position: {1}.";

        public static readonly string MissingOperationParameter =
            "Operation parameter is missing. Position: \"{0}\".";

        public static readonly string MissingEndOfScope =
            "Closing token is missing: \"{0}\".";

        public static readonly string UnexpectedEndOfPattern =
            "Unexpected end of pattern.";
    }
}
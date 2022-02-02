using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader
{
    internal static class ErrorMessages
    {
        public static class Common
        {
            public static readonly string NotSupported =
                "{0} is not supported.";

            public static readonly string DebugFailure =
                "Unexpected error!";
        }

        public static class Reader
        {
            public static readonly string GenericError =
                "Failed to complete reading.";

            public static readonly string EndOfText =
                "Unexpected end of text.";

            public static readonly string InfiniteLoop =
                $"{nameof(TextReaderWrapper)} has entered an infinite loop.";

            public static readonly string Position =
                "Position in source text: {0}.";

            public static readonly string Operation =
                "Operation: \"{0}\".";
        }

        public static class Pattern
        {
            public static readonly string GenericError =
                "Pattern is invalid.";

            public static readonly string Position =
                "Position in pattern: {0}.";
        }

        public static class Validation
        {
            public static readonly string GenericError =
                "Operation tree is invalid.";

            public static readonly string MissingRoot =
                $"{nameof(OperationTree)} must be the outermost operation.";

            public static readonly string DuplicateRoot =
                $"{nameof(OperationTree)} must not be repeated.";

            public static readonly string NestedContinuousOperationBlock =
                $"{nameof(ContinuousOperationBlock)} must not be nested in other operation blocks.";

            public static readonly string ContinuousOperationBlockFollowedByAnotherOperation =
                $"{nameof(ContinuousOperationBlock)} must not be followed by another operation.";

            public static readonly string EmptyCompositeOperation =
                "{0} must have at least one child operation.";
        }
    }
}
namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal static class Tokens
    {
        public const char OperationBlockScopeBeginning = '(';
        public const char OperationBlockScopeEnding = ')';
        public const char NumberOfRepetitionsScopeBeginning = '{';
        public const char NumberOfRepetitionsScopeEnding = '}';
        public const char CharBlockSizeScopeBeginning = '[';
        public const char CharBlockSizeScopeEnding = ']';
        public const char BoundaryStringSequenceScopeBeginning = '[';
        public const char BoundaryStringSeparator = '?';
        public const char BoundaryStringSequenceScopeEnding = ']';
        public const char CaseSensitiveBoundaryStringQuotationMark = '\'';
        public const char CaseInsensitiveBoundaryStringQuotationMark = '~';
        public const char RegexBoundaryStringQuotationMark = '/';
        public const char ContinuousOperationBlockPostfix = '*';
        public const char ReadOperation = 'R';
        public const char SkipOperation = 'S';
        public const char OneCharOperationQualifier = '.';
        public const char RemainingLineOperationQualifier = '>';
        public const char BoundaryStringOperationNoOversteppingQualifier = '|';
        public const char BoundaryStringOperationWithOversteppingQualifier = '+';
        public const char OperationParameterScopeBeginning = '{';
        public const char OperationParameterPrefix = '&';
        public const char OperationParameterScopeEnding = '}';
        public const char Escape = '\\';
        public const char CarriageReturn = 'r';
        public const char LineFeed = 'n';
        public const char Minus = '+';
        public const char Plus = '-';
    }
}
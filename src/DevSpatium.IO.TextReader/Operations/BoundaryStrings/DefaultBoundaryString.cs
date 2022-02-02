using System;

namespace DevSpatium.IO.TextReader.Operations
{
    public sealed class DefaultBoundaryString : IBoundaryString
    {
        private readonly StringComparison _stringComparison;

        public DefaultBoundaryString(string value, bool ignoreCase, TextComparison textComparison)
        {
            Value = value.NotNullOrEmpty(nameof(value));
            textComparison.AssertDefined(nameof(textComparison));
            _stringComparison = MapToStringComparison(textComparison, ignoreCase);
            IgnoresCase = ignoreCase;
        }

        public bool IgnoresCase { get; }
        public string Value { get; }

        public BoundaryStringMatch Match(string text)
        {
            text.NotNullOrEmpty(nameof(text));
            var index = text.IndexOf(Value, _stringComparison);
            if (index == -1)
            {
                return BoundaryStringMatch.Failed();
            }

            var matchingValue = text.Substring(index, Value.Length);
            return new BoundaryStringMatch(true, matchingValue, index);
        }

        private static StringComparison MapToStringComparison(TextComparison textComparison, bool ignoreCase)
        {
            if (textComparison == TextComparison.IgnoreCulture)
            {
                return ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            }

            return ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
        }
    }
}
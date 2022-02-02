using System.Text.RegularExpressions;

namespace DevSpatium.IO.TextReader.Operations
{
    public sealed class RegexBoundaryString : IBoundaryString
    {
        private readonly Regex _regex;

        public RegexBoundaryString(string regexPattern, TextComparison textComparison)
        {
            RegexPattern = regexPattern.NotNullOrEmpty(nameof(regexPattern));
            textComparison.AssertDefined(nameof(textComparison));
            var regexOptions = RegexOptions.Compiled;
            if (textComparison == TextComparison.IgnoreCulture)
            {
                regexOptions |= RegexOptions.CultureInvariant;
            }

            _regex = new Regex(regexPattern, regexOptions, Regex.InfiniteMatchTimeout);
        }

        public string RegexPattern { get; }

        public BoundaryStringMatch Match(string text)
        {
            text.NotNullOrEmpty(nameof(text));
            var regexMatch = _regex.Match(text);
            return new BoundaryStringMatch(regexMatch.Success, regexMatch.Value, regexMatch.Index);
        }
    }
}
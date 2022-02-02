using System.Collections;
using System.Collections.Generic;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    public sealed class CustomPatternParsers : ICustomOperationParsers
    {
        private static readonly List<ICustomOperationParser> Parsers;

        static CustomPatternParsers()
        {
            Parsers = new List<ICustomOperationParser>();
            Instance = new CustomPatternParsers();
        }

        private CustomPatternParsers()
        {
        }

        public static CustomPatternParsers Instance { get; }
        public int Count => Parsers.Count;

        public CustomPatternParsers Register(ICustomOperationParser parser)
        {
            parser.NotNull(nameof(parser));
            if (!Parsers.Contains(parser))
            {
                Parsers.Add(parser);
            }

            return this;
        }

        public IEnumerator<ICustomOperationParser> GetEnumerator()
            => ((IEnumerable<ICustomOperationParser>)Parsers).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
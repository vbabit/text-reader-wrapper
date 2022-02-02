using System.Collections.Generic;
using System.Linq;
using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal class CompositePatternParser : IPatternParser
    {
        private readonly List<IPatternParser> _parsers = new List<IPatternParser>();

        public virtual bool CanParse(IPatternReader reader)
        {
            reader.NotNull(nameof(reader));
            return _parsers.Any(parser => parser.CanParse(reader));
        }

        public virtual void Parse(IPatternReader reader, CompositeOperation operations)
        {
            reader.NotNull(nameof(reader));
            operations.NotNull(nameof(operations));
            var currentParser = GetParser(reader);
            while (currentParser != null)
            {
                currentParser.Parse(reader, operations);
                currentParser = GetParser(reader);
            }
        }

        public CompositePatternParser Add(IPatternParser parser)
        {
            parser.NotNull(nameof(parser));
            if (!_parsers.Contains(parser))
            {
                _parsers.Add(parser);
            }

            return this;
        }

        private IPatternParser GetParser(IPatternReader reader)
            => _parsers.FirstOrDefault(parser => parser.CanParse(reader));
    }
}
using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal interface IPatternParser
    {
        bool CanParse(IPatternReader reader);
        void Parse(IPatternReader reader, CompositeOperation operations);
    }
}
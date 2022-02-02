using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal interface IPatternPositionRegistry
    {
        void Register(IOperation operation, PatternPosition position);
        void Clear();
    }
}
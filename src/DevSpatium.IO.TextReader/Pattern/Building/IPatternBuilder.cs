using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Building
{
    internal interface IPatternBuilder
    {
        string Build(IOperation operation);
    }
}
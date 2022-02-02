using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Building
{
    public interface ICustomPatternBuilder
    {
        bool IsOperationSupported(IOperation operation);
        string Build(IOperation operation);
    }
}
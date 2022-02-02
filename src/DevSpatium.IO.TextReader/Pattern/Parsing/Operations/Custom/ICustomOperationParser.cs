using DevSpatium.IO.TextReader.Operations;

namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    public interface ICustomOperationParser
    {
        bool IsOperationSupported(char qualifyingToken);
        BaseOperation Parse(IPatternReader reader, OperationType operationType);
    }
}
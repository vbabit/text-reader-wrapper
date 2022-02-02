namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class DefaultOperationBlockScope : PatternScope
    {
        public DefaultOperationBlockScope()
            : base(Tokens.OperationBlockScopeBeginning, Tokens.OperationBlockScopeEnding)
        {
        }
    }
}
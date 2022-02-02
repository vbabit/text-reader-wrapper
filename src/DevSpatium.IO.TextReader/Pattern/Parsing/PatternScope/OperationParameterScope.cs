namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class OperationParameterScope : PatternScope
    {
        public OperationParameterScope()
            : base(Tokens.OperationParameterScopeBeginning, Tokens.OperationParameterScopeEnding)
        {
        }

        public override bool CanEnter(IPatternReader reader)
            => reader.NotNull(nameof(reader)).CheckNextToken(Tokens.OperationParameterScopeBeginning);

        public override IPatternScope Enter(IPatternReader reader)
        {
            reader.NotNull(nameof(reader));
            base.Enter(reader);
            reader.ReadAndAssert(Tokens.OperationParameterPrefix);
            return this;
        }
    }
}
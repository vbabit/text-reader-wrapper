namespace DevSpatium.IO.TextReader.Pattern.Parsing
{
    internal sealed class OperationParameterParser : IOperationParameterParser
    {
        private readonly IPatternScope _innerScope = new OperationParameterScope();
        public bool CanParse(IPatternReader reader) => _innerScope.CanEnter(reader);

        public char Parse(IPatternReader reader)
        {
            reader.NotNull(nameof(reader));
            _innerScope.Enter(reader);
            var nextToken = reader.Peek();
            if (nextToken == _innerScope.ClosingToken)
            {
                throw ParsingErrors.MissingOperationParameter(reader.Position);
            }

            var parameter = reader.Read();
            _innerScope.Exit(reader);
            return parameter;
        }
    }
}